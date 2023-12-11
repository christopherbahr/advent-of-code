// There are probably a couple viable approaches here
// Probably the most efficient would be to iterate the grid 
// looking for symbols and then "grow" numbers from the touching 
// digits. 
//
// Given the helpers we have though that are so good at finding whole ints
// and are good at finding neighbors of things in grids it seemed easier to first
// lecate and track all the ints that are touching any symbol and then iterate
// the stars in the grid and check which tracked ints are close to them.

namespace AOC23;
public class Day3 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var cg = lines.AsCharGrid();
                var intsWithLocations = new Dictionary<int, IList<(int sc, int ec, int val)>>();
                var p1 = 0;

                foreach(var (i, line) in lines.Enumerate())
                {
                        // each of these is a digit that is adjacent to a synmbol
                        // more than one digit from the same number may be next to a symbol
                        var allInts = line.GetInts().Select(x => Math.Abs(x));
                        var curIdx = 0;
                        foreach(var val in allInts)
                        {
                                // Find the integer in the line
                                var asStr = val.ToString();
                                var idx = line.IndexOf(asStr, curIdx);
                                foreach(var (j, c) in asStr.Enumerate())
                                {
                                        var shouldAdd= (i, j + idx).allDirs(cg)
                                        .Where(x => !cg[x.cr, x.cc].IsDigit() && cg[x.cr, x.cc] != '.').Any();
                                        if(shouldAdd)
                                        {
                                                p1 += val;
                                                var toAdd = (idx, idx + asStr.Length - 1, val);
                                                intsWithLocations.AddToList(i, toAdd);
                                                break;
                                        }
                                }
                                curIdx = idx + asStr.Length;
                        }
                }

                // In the first step we found all the cansidate integers and recorded where they are in the line.
                // Now find the stars and check if they are "touching" any numbers
                var p2 = 0L;
                
                foreach(var ((cr, cc), v) in cg.Enumerate())
                {
                        if(v != '*')
                        {
                                continue;
                        }

                        var touchingNums = new List<int>();
                        foreach(var ccr in Enumerable.Range(cr -1, 3))
                        {
                                if(!intsWithLocations.TryGetValue(ccr, out var candidates))
                                {
                                        continue;
                                }
                                foreach(var (sc, ec, val) in candidates)
                                {
                                        // We already know that theRow is touching. Check thEColums
                                        if (sc <= cc + 1 && ec >= cc - 1)
                                        {
                                                touchingNums.Add(val);
                                        }
                                }
                        }
                        if(touchingNums.Count == 2)
                        {
                                p2 += touchingNums[0] * touchingNums[1];
                        }
                }

                Console.WriteLine((p1, p2));
        }
}
