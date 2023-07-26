// This implementation is left as the original implementation that I actually got working
// The search space is too big to just search so I had to find some way to restrict it
// The first major insight is that each input is treated separately. so we process chunk by chunk
// The first implementation of that had a cache, the end one didn't but maybe should have?
// 
// The real way I got it trimmed down though was by finding some of the recurrence relations
// that we needed to hit in order to get an acceptable input number. Since those go digit by digit
// it imposed some restrictions on the possible number. For example index 4 has to be 2 bigger than
// index 5. That means that index 4 must be at least 3 and index 5 must be at most 7. Any time as 
// as we're iterating down or up that the relationship or the rules aren't respected we can jump way
// ahead in our guess. Cutting off tons of numbers. I wasn't able to work out all the relationships
// this simply so several I just ignored. Just using the simple ones was enough though.
//
// Still this was very slow and very finnicky to get right. The new backtracking implementaiton is 
// much faster, much more understandable, and doesn't rely on any translation of the input data to
// native c# (though it would probably be faster still if it did).


using System.Numerics;

namespace AOC21;
public class Day24Original
{
        public List<Func<int, BigInteger, BigInteger>> funcs = new List<Func<int, BigInteger, BigInteger>>();

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                for(var i = 0; i< 14; i++)
                {
                        cache[i] = new Dictionary<(int, BigInteger), BigInteger>();
                };
                var lines = File.ReadAllLines("wip.in");
                var lineChunks = new List<List<string>>();
                var curLine = new List<string>();
                foreach(var line in lines.Skip(1))
                {
                        if(line.Split(' ').First() == "inp")
                        {
                                lineChunks.Add(curLine);
                                curLine = new List<string>();
                        }
                        else
                        {
                                curLine.Add(line);
                        }
                }
                lineChunks.Add(curLine);

                if(lineChunks.Count() != 14)
                {
                        Console.WriteLine("bad chunks");
                }

                funcs.AddRange(new [] {f0, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13});

                var chunkArray = lineChunks.Select(x => x.ToArray()).ToArray();

                var max = 99_999_999_999_999L;
                var counter = 0L;
                BigInteger minSoFar = -1;
                BigInteger basez = 0;
                // Since the first "rule" is the relationship between w4 and w5 (w4 - 2 = w5)
                // let's assume that w0-w3 can all just be 9s
                // since we're trying to find the biggest number
                 // Still too slow
                 // Let's go a step further and assume it starts 999997 which is the highest number that obeys the first rule
                 var tryArr = new [] { 1, 1, 1, 1, 3, 1};
                 var tryCount = 0;
                for (var j = 0; j < tryCount; j++)
                {
                        basez = ProcessChunk(j, tryArr[j], basez);
                }
                var random = new Random();
                Console.WriteLine(basez);
                var dist = 0;
                var p1 = 0L;
                var p2 = 0L;
                for(var i = max; i >= 10_000_000_000_000L; i--)
                {
                        // TODO: This has to be super slow..
                        var asArr = i.ToString().ToCharArray().Select(x => x - '0').ToArray();
                        if(asArr[13] == 0)
                        {
                                continue;
                        }
                        

                        if(asArr[4] <= 2)
                        {
                                var toSub = i % 1_000_000_000;
                                i -= toSub;
                                continue;
                        }
                        if(asArr[5] > 7)
                        {
                                var toSub = (i % 1_000_000_000) - 800_000_000; // usually 199_999_999
                                i -= toSub;
                                continue;
                        }
                        dist = asArr[4] - asArr[5];
                        if(dist != 2)
                        {
                                i -= 999_999_999;
                                continue;
                        }
                        //asArr[5] = asArr[4] - 2;


                        if(asArr[7] > 5)
                        {
                                var toSub = (i % 10_000_000) - 6_000_000; // usually 3,999,999
                                i -= toSub;
                                continue;
                        }
                        if(asArr[8] < 5)
                        {
                                var toSub = i % 1_000_000;
                                i -= toSub;
                                continue;
                        }
                        dist = asArr[8] - asArr[7];
                        if(dist != 4)
                        {
                                i -= 999_999;
                                continue;
                        }
                        //asArr[8] = asArr[7] + 4;


                        if(asArr[9] <= 6)
                        {
                                var toSub = i % 100_000;
                                i -= toSub;
                                continue;
                        }
                        if(asArr[10] > 3)
                        {
                                // skip ahead until we're at least 3
                                var toSub = (i % 10_000) - 4000;
                                i -= toSub;
                                continue;
                        }
                        dist = asArr[9] - asArr[10];
                        if(dist != 6)
                        {
                                // If the distance is too far jump 1000 forward
                                i -= 9_999;
                                continue;
                        }
                        

                        if(asArr.Any(x => x == 0))
                        {
                                continue;
                        } 
                        BigInteger altz = basez;
                        for(var j = tryCount; j < 14; j++)
                        {
                                var w = 0;
                                w = asArr[j - tryCount];
                                altz = ProcessChunk(j, w, altz);
                        }
                        if(minSoFar == -1)
                        {
                                minSoFar = altz;
                        }
                        if(altz < minSoFar)
                        {
                                minSoFar = altz;
                                Console.WriteLine((string.Join(string.Empty, asArr), minSoFar));
                        }
                        if(altz == 0)
                        {
                                p1 = i;
                                break;
                        }
                        counter++;
                        if(counter % 5_000_000 == 0)
                        {
                                Console.WriteLine((i, counter));
                        }
                }

                Console.WriteLine($"p1: {p1}");

                var min = 11_111_111_111_111;

                for(var i = min; i <= max; i++)
                {
                        // TODO: This has to be super slow..
                        var asArr = i.ToString().ToCharArray().Select(x => x - '0').ToArray();
                        if(asArr[13] == 0)
                        {
                                continue;
                        }
                        

                        if(asArr[4] <= 2)
                        {
                                var toAdd = 3_111_111_110 - (i % 1_000_000_000);
                                i += toAdd;
                                continue;
                        }
                        if(asArr[5] > 7)
                        {
                                var toAdd = 1_111_111_110 - (i % 1_000_000_000); // usually 300_000_000
                                i += toAdd;
                                continue;
                        }
                        dist = asArr[4] - asArr[5];
                        if(dist != 2)
                        {
                                i += 999_999_999;
                                continue;
                        }
                        //asArr[5] = asArr[4] - 2;

// TODO: Add This Constraint To p1
                        if(asArr[6] < 8)
                        {
                                var toAdd = 81_111_110 - (i % 100_000_000);
                                i += toAdd;
                                continue;
                        }


                        if(asArr[7] > 5)
                        {
                                var toAdd = 11_111_110 - (i % 10_000_000); // usually 3,999,999
                                i += toAdd;
                                continue;
                        }
                        if(asArr[8] < 5)
                        {
                                var toAdd = 511_110- (i % 1_000_000);
                                i += toAdd;
                                continue;
                        }
                        dist = asArr[8] - asArr[7];
                        if(dist != 4)
                        {
                                i += 999_999;
                                continue;
                        }
                        //asArr[8] = asArr[7] + 4;

                        if(asArr[9] <= 6)
                        {
                                var toAdd = 71_110 - (i % 100_000);
                                i += toAdd;
                                continue;
                        }
                        if(asArr[10] > 3)
                        {
                                // skip ahead until we're at least 3
                                var toAdd = 11_110 - (i % 10_000);
                                i += toAdd;
                                continue;
                        }
                        dist = asArr[9] - asArr[10];
                        if(dist != 6)
                        {
                                // If the distance is too far jump 1000 forward
                                i += 9_999;
                                continue;
                        }
                        

                        if(asArr.Any(x => x == 0))
                        {
                                continue;
                        } 
                        BigInteger altz = basez;
                        for(var j = tryCount; j < 14; j++)
                        {
                                var w = 0;
                                w = asArr[j - tryCount];
                                altz = ProcessChunk(j, w, altz);
                        }
                        if(minSoFar == -1)
                        {
                                minSoFar = altz;
                        }
                        if(altz < minSoFar)
                        {
                                minSoFar = altz;
                                Console.WriteLine((string.Join(string.Empty, asArr), minSoFar));
                        }
                        if(altz == 0)
                        {
                                p2 = i;
                                break;
                        }
                        counter++;
                        if(counter % 5_000_000 == 0)
                        {
                                Console.WriteLine((i, counter));
                        }
                }

                Console.WriteLine(p2);
        }

        public bool Compare(string f1, string f2)
        {
                var l1 = File.ReadAllLines(f1);
                var l2 = File.ReadAllLines(f2);
                var r = new Random();

                for(var i = 0; i < 10000; i++)
                {
                        int[] num = new int[14];
                        for(var j = 0; j < 14; j++)
                        {
                                num[j] = r.Next(1, 9);
                        }

                        var o1 = IsValidNumber(num, l1);
                        var o2 = IsValidNumber(num, l2);
                        if(o1 != o2)
                        {
                                Console.WriteLine("BUG");
                                return false;
                        }
                }
                Console.WriteLine("Good");
                return true;
        }

        private Dictionary<int, Dictionary<(int, BigInteger), BigInteger>> cache = new Dictionary<int, Dictionary<(int, BigInteger), BigInteger>>();

        public BigInteger f0(int w, BigInteger z)
        {
                var idx = 0;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var val = w + 2;
                cache[idx][key] = val;
                return val;
        }

        public BigInteger f1(int w, BigInteger z)
        {
                var idx = 1;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 4;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f2(int w, BigInteger z)
        {
                var idx = 2;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 8;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f3(int w, BigInteger z)
        {
                var idx = 3;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 7;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f4(int w, BigInteger z)
        {
                var idx = 4;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 12;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f5(int w, BigInteger z)
        {
                var idx = 5;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 14;
                if(x == w)
                {
                       ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 7;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f6(int w, BigInteger z)
        {
                var idx = 6;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26);
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 10;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f7(int w, BigInteger z)
        {
                var idx = 7;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 14;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f8(int w, BigInteger z)
        {
                var idx = 8;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 10;
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 2;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f9(int w, BigInteger z)
        {
                var idx = 9;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                var ret = z * 26;
                ret = ret + w + 6;
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f10(int w, BigInteger z)
        {
                var idx = 10;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 12;
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 8;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f11(int w, BigInteger z)
        {
                var idx = 11;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 3;
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 11;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f12(int w, BigInteger z)
        {
                var idx = 12;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 11;
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 5;
                }
                cache[idx][key] = ret;
                return ret;
        }

        public BigInteger f13(int w, BigInteger z)
        {
                var idx = 13;
                var key = (w, z);
                if(cache[idx].ContainsKey(key))
                {
                        return cache[idx][key];
                }
                BigInteger ret;
                var x = (z % 26) - 2;
                if(x == w)
                {
                        ret = z / 26;
                }
                else
                {
                        ret = ((z / 26) * 26) + w + 11;
                }
                cache[idx][key] = ret;
                return ret;
        }
        
        public BigInteger ProcessChunk(int chunkidx, int digit, BigInteger z)
        {
                return funcs[chunkidx](digit, z);
        }

        //  Key Observation:
        // Treat each number independently
        // Each time we read the x, y, w are reset so only z remains
        // so at the point of the second read 98 is the same as 97
        public Dictionary<string, BigInteger> cl = new Dictionary<string, BigInteger>();
        public BigInteger ProcessChunkLegacy(string[] instructions, int chunkidx, int digit, BigInteger z)
        {
                // Whenever we have one that has a div that's 
                // supposed to be a pop.
                // If we fail to pop in that case we should throw
                var gotPop = false;
                var hadDiv26 = false;
                var str = $"{chunkidx}-{digit}-{z}";
                if(cl.ContainsKey(str))
                {
                        if(cl[str] == int.MinValue)
                        {
                                throw new Exception("cached bad path");
                        }
                        return cl[str];
                }
                var registers = new Dictionary<string, BigInteger>();
                registers["w"] = digit;
                registers["x"] = 0;
                registers["y"] = 0;
                registers["z"] = z;
                foreach(var inst in instructions)
                {
                        var data = inst.Split(' ');
                        switch(data[0])
                        {
                                case "#": //comment
                                        continue;
                                // We'll cheat here since inp always goes to w
                                case "inp":
                                        throw new Exception("bang");
                                case "add":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 + v2;
                                        break;
                                }
                                case "mul":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 * v2;
                                        break;
                                }
                                case "div":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        if(v2 == 26)
                                        {
                                                hadDiv26 = true;
                                        }
                                        registers[data[1]] = v1 / v2;
                                        break;
                                }
                                case "mod":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 % v2;
                                        break;
                                }
                                case "eql":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        if(v1 == v2 && data[2] == "w")
                                        {
                                                gotPop = true;
                                        }
                                        registers[data[1]] = (v1 == v2) ? 1 : 0;
                                        break;
                                }
                        }

                }

                if(hadDiv26 && !gotPop)
                {
                        cl[str] = int.MinValue;
                        throw new Exception("Bad Path");
                }

                cl[str] = registers["z"];
                return registers["z"];
        }

        public BigInteger IsValidNumber(int[] numbers, string[] instructions)
        {
                var registers = new Dictionary<string, BigInteger>();
                registers["w"] = 0;
                registers["x"] = 0;
                registers["y"] = 0;
                registers["z"] = 0;
                var i = 0;
                foreach(var inst in instructions)
                {
                        var data = inst.Split(' ');
                        switch(data[0])
                        {
                                case "#": //comment
                                        continue;
                                // We'll cheat here since inp always goes to w
                                case "inp":
                                        registers["w"] = numbers[i];
                                        i++;
                                        break;
                                case "add":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 + v2;
                                        break;
                                }
                                case "mul":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 * v2;
                                        break;
                                }
                                case "div":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 / v2;
                                        break;
                                }
                                case "mod":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 % v2;
                                        break;
                                }
                                case "eql":
                                {
                                        var v1 = registers[data[1]];
                                        if(!BigInteger.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = (v1 == v2) ? 1 : 0;
                                        break;
                                }
                        }

                }

                return registers["z"];
        }

}