namespace AOC22;
public class Day17 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                long p1 = 0;

                int[,] rock1 = new int[4,4] {
                        { 0, 0, 0, 0},
                        { 0, 0, 0, 0},
                        { 0, 0, 0, 0},
                        { 1, 1, 1, 1},
                };

                int[,] rock2 = new int[4,4] {
                        { 0, 0, 0, 0},
                        { 0, 1, 0, 0},
                        { 1, 1, 1, 0},
                        { 0, 1, 0, 0},
                };

                int[,] rock3 = new int[4,4] {
                        { 0, 0, 0, 0},
                        { 0, 0, 1, 0},
                        { 0, 0, 1, 0},
                        { 1, 1, 1, 0},
                };

                int[,] rock4 = new int[4,4] {
                        { 1, 0, 0, 0},
                        { 1, 0, 0, 0},
                        { 1, 0, 0, 0},
                        { 1, 0, 0, 0},
                };

                int[,] rock5 = new int[4,4] {
                        { 0, 0, 0, 0},
                        { 0, 0, 0, 0},
                        { 1, 1, 0, 0},
                        { 1, 1, 0, 0},
                };

                var hTower = new HashSet<(long r, int c)>();
                //var tower = new int[2022*4,7];

                var rocks = new List<int[,]>{rock1, rock2, rock3, rock4, rock5};

                var input = lines.Single();
                var directions = input.Select(x => x == '<' ? -1 : 1).ToList();

                var loopCount = directions.Count * 5;

                var rockCount = 1000000000000;
                //var rockCount = 10000000;
                //var rockCount = 6407785;
                //var rockCount = 2022;
                var useCache = true;

                long height = 0;
                long counter = 0;
                // The trick is that it loops so we find the height at the end of one loop

                Console.WriteLine($"LoopCount = {loopCount}");

                var patternCache = new Dictionary<string, (long, long)>();

                var lastPattern = new long[7];
                var nextPattern = new long[7];

                var towerPeaks = new long[7];

                var towerCache = hTower;

                var rockCounterPairs = new HashSet<(long, long)>();

                // The basic idea is to record the peaks after every step
                // when we get a repeat rock, direction, peak set then we can jump the simulation forward based on what happened last time

                // so we record every rock, direction index pair, and keep a cache of pattern -> heigt + idx jumps
                // then when we find a repeat we just do the same thing as happened last time
                //
                // I had some fears about "sneak throughs" where things could sneak through the cracks if we just recorded
                // the peaks but I think given the scale of the chunks not enough things can sneak through. 

                for(long i = 0; i < rockCount; i++)
                {
                        // we potentially have a loop
                        if(rockCounterPairs.Contains((i%5, counter % directions.Count)) && towerPeaks.All( x=> x> 0) && useCache)
                        {
                                // we have a viable loop if we have an identical contiguous surface to a previous loop point
                                // a contiguous surface is a path from wall to wall that never leaves touching rocks

                                var normalizedPeaks = towerPeaks.Select(x => height - x).ToList();

                                var key = string.Join(',', normalizedPeaks) + '.' + i % 5 + '.' + counter % directions.Count;

                                if(patternCache.ContainsKey(key))
                                {
                                        var (lastIdx, lastHeight) = patternCache[key];

                                        // record the amount we've jumped in index and then the amount
                                        // we've increased in height
                                        var idxIncrease = i - lastIdx;
                                        var heightIncrease = height - lastHeight;
                                        
                                        // This is how many jumps of this size we can make
                                        var remainingLoops = (rockCount - i) / idxIncrease;
                                        var leftovers = (rockCount - i) % idxIncrease;
                                        Console.WriteLine($"Height before jump: {height} {i}");
                                        var addToHeight = heightIncrease * remainingLoops;
                                        height += addToHeight;

                                        Console.WriteLine($"Hit, {key} {idxIncrease} {heightIncrease} {remainingLoops} {leftovers}");

                                        i = rockCount - leftovers;
                                        // Add the peaks back, does not guarantee stopping though, something could slip through?
                                        for(var pc = 0; pc < towerPeaks.Length; pc++)
                                        {
                                                hTower.Add((towerPeaks[pc] + addToHeight, pc));
                                        }
                                        Console.WriteLine($"Height after jump: {height} {i}");
                                }
                                else
                                {
                                        if(useCache)
                                        {
                                                Console.WriteLine($"No pattern match: {key}");
                                        }
                                }

                                // Record the pattern, what index we were at, and what the height was
                                patternCache[key] = (i, height);
                        }

                        var rock = rocks[(int)(i % 5)];
                        var landed = false;
                        var (cc, cr) = (2, height + 3);

                        while(!landed)
                        {
                                rockCounterPairs.Add((i%5, counter % directions.Count));
                                var dir = directions[(int)(counter % directions.Count)];
                                counter++;

                                // pushed by gas
                                var tc = cc + dir;
                                if(tc >= 0 && tc < 7 && CanMoveRock(rock, tc, cr, hTower))
                                {
                                        cc = tc;
                                }

                                var tr = cr - 1;

                                // we've fallen off the end of usefulness here
                                if(height - tr > 1000)
                                {
                                        landed = true;
                                        continue;
                                }

                                if(tr < 0 || !CanMoveRock(rock, cc, tr, hTower))
                                {
                                        long highest = 0;
                                        landed = true;
                                        for (int rr = 3; rr >= 0; rr--)
                                        {
                                                for (int rc = 0; rc < 4; rc++)
                                                {
                                                        if(rock[rr, rc] == 1)
                                                        {
                                                                long h = cr + (3 - rr);
                                                                highest = Math.Max(h, highest);
                                                                if(h > towerPeaks[cc + rc])
                                                                {
                                                                        towerPeaks[cc + rc] = h;
                                                                }
                                                                //tower[cr + (3 - rr), cc + rc] = 1;
                                                                hTower.Add((cr + (3 - rr), cc + rc));
                                                        }
                                                        else
                                                        {
                                                                continue;
                                                        }
                                                }
                                        }
                                        height = Math.Max(height, highest + 1);
                                }
                                else
                                {
                                        cr = tr;
                                }
                        }
                        if(i == 2021)
                        {
                                p1 = height;
                        }
                }

                Console.WriteLine(height);
                
                Console.WriteLine( (p1.ToString(), height.ToString()));
        }

        private bool CanMoveRock1(int[,] rock, int cc, long cr, int[,] tower)
        {
                //cc, cr track the bottom left corner of the rock
                if(cr < 0)
                {
                        return false;
                }

                for (int rr = 3; rr >= 0; rr--)
                {
                        for (int rc = 0; rc < 4; rc++)
                        {
                                var rockPresent = rock[rr, rc] == 1;
                                // bounds of the rock are outside the right edge
                                // but the rock may not be that wide
                                if(cc + rc >= 7)
                                {
                                        if(rockPresent)
                                        {
                                                return false;
                                        }
                                        else
                                        {
                                                continue;
                                        }
                                }
                                var towerPresent = tower[cr + (3 - rr), cc + rc] == 1;
                                if(rockPresent && towerPresent)
                                {
                                        return false;
                                }
                        }
                }

                return true;
        }

        private bool CanMoveRock(int[,] rock, int cc, long cr, HashSet<(long, int)> tower)
        {
                //cc, cr track the bottom left corner of the rock
                if(cr < 0)
                {
                        return false;
                }

                for (int rr = 3; rr >= 0; rr--)
                {
                        for (int rc = 0; rc < 4; rc++)
                        {
                                var rockPresent = rock[rr, rc] == 1;
                                // bounds of the rock are outside the right edge
                                // but the rock may not be that wide
                                if(cc + rc >= 7)
                                {
                                        if(rockPresent)
                                        {
                                                return false;
                                        }
                                        else
                                        {
                                                continue;
                                        }
                                }
                                var towerPresent = tower.Contains((cr + (3 - rr), cc + rc));
                                if(rockPresent && towerPresent)
                                {
                                        return false;
                                }
                        }
                }

                return true;
        }
}