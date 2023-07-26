namespace AOC21;
public class Day8 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var outputList = new List<List<string>>();

                foreach(var line in lines)
                {
                        var parts = line.Split('|');
                        outputList.Add(parts[1].Split(' ').ToList());
                }

                var easyCount = 0;
                foreach(var entry in outputList.SelectMany(x => x))
                {
                        // check for 1, 4, 7, 8
                        if(entry.Length == 2 || entry.Length == 4 || entry.Length == 3 || entry.Length == 7)
                        {
                                easyCount++;
                        }
                }
                Console.WriteLine(easyCount);

                var bigSum = 0;
                foreach(var line in lines)
                {
                        var parts = line.Split('|');
                        var map = BuildMap(parts[0].Split(' '));
                        foreach(var entry in map)
                        {
                        }
                        var outputPart = parts[1].Split(' ');
                        var numStr = "";
                        foreach(var part in outputPart.Where(x => x.Length > 0).Select(x => x.Select(y => map[y])))
                        {
                                var sortedPart = new string(part.OrderBy(x => x).ToArray());
                                var num = display[sortedPart];
                                numStr += num;
                        }
                        bigSum += int.Parse(numStr);
                }
                Console.WriteLine(bigSum);
        }

        private Dictionary<string, int> display = new Dictionary<string, int>
        {
                ["abcefg"] = 0,
                ["cf"] = 1,
                ["acdeg"] = 2,
                ["acdfg"] = 3,
                ["bcdf"] = 4,
                ["abdfg"] = 5,
                ["abdefg"] = 6,
                ["acf"] = 7,
                ["abcdefg"] = 8,
                ["abcdfg"] = 9
        };

        private Dictionary<char, char> BuildMap(IEnumerable<string> input)
        {
                var chars = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g'};
                char a, b, c, d, e, f, g = 'Z';

                // first use number 1 to find f & c
                var poss = input.First( x=> x.Length == 2);

                // now use 7 to find a by finding 7 and removing the 2 that we know must be c and f
                a = input.First( x=> x.Length == 3).Except(poss.ToCharArray()).Single();
                
                // only 0, 6, and 9 have 6 letters, they both use f but don't both use c
                // if both 6 character numbers have the letter than it must be f, otherwise it's c

                var isFFirst = input.Where(x => x.Length == 6).All(x => x.Contains(poss[0]));
                if(isFFirst)
                {
                        f = poss[0];
                        c = poss[1];
                }
                else
                {
                        f = poss[1];
                        c = poss[0];
                }

                // Now use 4 and the 6 char inputs to figure out b & d
                // b & d will be 4 without c & f. All 6 char numbers use b but 0 doesn't use d
                var bdPoss = input.First( x => x.Length == 4).Except(poss).ToArray();

                var isBFirst = input.Where(x => x.Length == 6).All(x => x.Contains(bdPoss[0]));
                if(isBFirst)
                {
                        b = bdPoss[0];
                        d = bdPoss[1];
                }
                else
                {
                        b = bdPoss[1];
                        d = bdPoss[0];
                }

                // now eg poss is just what's left

                var egPoss = chars.Except(poss).Except(bdPoss).Except(new [] {a}).ToArray();

                // same type of trick as before. All of 2, 3, 5 have 5 chars and use G but they don't all use
                // e 

                var isGFirst = input.Where(x => x.Length == 5).All(x => x.Contains(egPoss[0]));
                if(isGFirst)
                {
                        g = egPoss[0];
                        e = egPoss[1];
                }
                else
                {
                        g = egPoss[1];
                        e = egPoss[0];
                }
                var rd = new Dictionary<char, char>{
                        [a] = 'a',
                        [b] ='b',
                        [c] ='c',
                        [d] ='d',
                        [e] ='e',
                        [f] ='f',
                        [g] ='g',
                };
                return rd;
        }

        private IEnumerable<(int, T)> Enumerate<T>(IEnumerable<T> input)
        {
                var i = 0;
                foreach(var thing in input)
                {
                        yield return (i, thing);
                        i++;
                }
        }


}