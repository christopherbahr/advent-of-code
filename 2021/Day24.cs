// This one got a significant rewrite to go from a DP approach to 
// a backtracking approach. The idea is that any time you fail to hit the
// "pop" condition where you end up dividing z but not multiplying
// you have failed and can backtrack.

// That approach combined with per-chunk caching gets the whole thing done
// in under a second.

// To improve more I think we could cheat and basically write the ALU as 
// actual c# code (one of the things I did to get the DP one fast enough to finish).
// It could be one function with a couple parameters.

// That would skip all the weird Dictionary based register manipulation and surely
// be faster than parsing strings.

// Still down under a second for now I wont bother.
namespace AOC21;
public class Day24 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
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

                var chunkArray = lineChunks.Select(x => x.ToArray()).ToArray();
                var digits = new int[14];
                var toTry = new [] { 9, 8 , 7, 6, 5, 4, 3, 2, 1};
                GetNumber(lineChunks, 0, 0, digits, toTry);
                Console.WriteLine($"BiggestNumber: {string.Join("", digits)}");
                digits = new int[14];
                GetNumber(lineChunks, 0, 0, digits, toTry.Reverse().ToArray());
                Console.WriteLine($"SmallestNumber: {string.Join("", digits)}");

        }

        public int GetNumber(List<List<string>> chunks, int chunkIdx, long z, int[] digits, int[] toTry)
        {
                var successNum = 0;
                var origZ = z;
                foreach(var attempt in toTry)
                {
                        // Throws if we miss a rule
                        var (processed, foundZ) = ProcessChunkLegacy(chunks[chunkIdx], chunkIdx, attempt, origZ);
                        if(!processed)
                        {
                                continue;
                        }
                        // Throws if we get to the end bul fail
                        if (chunkIdx == 13)
                        {
                                if(foundZ != 0)
                                {
                                        return -1;
                                }
                                else
                                {
                                        digits[13] = attempt;
                                        return 0;
                                }
                        }
                        else
                        {
                                var success = GetNumber(chunks, chunkIdx + 1, foundZ, digits, toTry);
                                if(success == -1)
                                {
                                        continue;
                                }
                        }
                        successNum = attempt;
                        break;
                }
                if(successNum == 0)
                {
                        return -1;
                }
                digits[chunkIdx] = successNum;
                return 0;
        }

        //  Key Observation:
        // Treat each number independently
        // Each time we read the x, y, w are reset so only z remains
        // so at the point of the second read 98 is the same as 97
        public Dictionary<long, long> cl = new Dictionary<long, long>();
        public (bool, long) ProcessChunkLegacy(IEnumerable<string> instructions, int chunkidx, int digit, long z)
        {
                // Whenever we have one that has a div that's 
                // supposed to be a pop.
                // If we fail to pop in that case we should throw
                var gotPop = false;
                var hadDiv26 = false;
                // add 3 0s on the right
                long idx = z * 1000;
                // use 2 zeros for chunk idx
                idx += chunkidx * 10;
                // use last 0 for digit
                idx += digit;
                if(cl.ContainsKey(idx))
                {
                        if(cl[idx] == int.MinValue)
                        {
                                return (false, 0);
                        }
                        return (true, cl[idx]);
                }
                var registers = new Dictionary<string, long>();
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
                                        if(!long.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 + v2;
                                        break;
                                }
                                case "mul":
                                {
                                        var v1 = registers[data[1]];
                                        if(!long.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 * v2;
                                        break;
                                }
                                case "div":
                                {
                                        var v1 = registers[data[1]];
                                        if(!long.TryParse(data[2], out var v2))
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
                                        if(!long.TryParse(data[2], out var v2))
                                        {
                                                v2 = registers[data[2]];
                                        }
                                        registers[data[1]] = v1 % v2;
                                        break;
                                }
                                case "eql":
                                {
                                        var v1 = registers[data[1]];
                                        if(!long.TryParse(data[2], out var v2))
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
                        cl[idx] = int.MinValue;
                        return (false, 0);
                }

                cl[idx] = registers["z"];
                return (true, registers["z"]);
        }
}