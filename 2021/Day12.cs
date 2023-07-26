// This is one of the first "slow" ones where execution takes more than a second
// Some dynamic programming might make things faster but I suspect the first thing to do
// is move from strings to ints as much as possible.
// Our starting point was ~2200 ms
// Something like: 
// 1. Number the nodes rather than using their names
//   1a. Numbers > 100 for capital letters
// Result: I left the Cave object alone but switched all ids and lookups to integers and direct list access
// that shaved off about 700ms
// 2. Bit field representing neighbors & visited
// Neighbors doesn't seem to help since we need to iterate
// visited probably does
// 3. Some simpler definition of path, maybe BigInteger with 2 bit ids or something
// Result: This worked and made things faster but involved so many BigIntegers that it may be faster
// to use a StringBuilder or some other more easile expandable thing
// Still we're well under a second now so I'lLl let this go for now
using System.Numerics;

namespace AOC21;
public class Day12 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var caves = new Dictionary<string, Cave>();


                foreach(var line in lines)
                {
                        var parts = line.Split('-');
                        var c1 = parts[0];
                        var c2 = parts[1];
                        if(!caves.ContainsKey(c1))
                        {
                                var cave = new Cave();
                                if(c1.ToUpper() == c1)
                                {
                                        cave.Big = true;
                                }
                                caves[c1] = cave;
                        }
                        if(!caves.ContainsKey(c2))
                        {
                                var cave = new Cave();
                                if(c2.ToUpper() == c2)
                                {
                                        cave.Big = true;
                                }
                                caves[c2] = cave;
                        }

                        caves[c1].Neighbors.Add(c2);
                        caves[c2].Neighbors.Add(c1);
                }

                // map from strings to ints for ids
                var caveMap = new Dictionary<string, int>();
                var startIdx = -1;
                var endIdx = -1;
                var caveList = new List<Cave>();

                foreach(var (i, (name, cave)) in caves.Enumerate())
                {
                        if(name == "start")
                        {
                                startIdx = i;
                        }
                        else if(name == "end")
                        {
                                endIdx = i;
                        }

                        caveMap[name] = i;
                        caveList.Add(cave);
                }
                foreach(var (i, (name, cave)) in caves.Enumerate())
                {
                        var nMask = 0;
                        foreach(var nCave in cave.Neighbors)
                        {
                                var num = caveMap[nCave];
                                cave.FastNeighbors.Add(num);
                                nMask += (1 << num);
                        }
                        cave.NeighborMask = nMask;
                }

                // Okay now we need to find all possible paths
                // that means doing a BFS with path and a strange visited collection where we only put small caves

                var q = new Queue<(int, int)>();
                
                q.Enqueue((startIdx, (1 << startIdx)));


                var pathCount = 0;
                while(q.Any())
                {
                        var (cs, v) = q.Dequeue();

                        if(cs == endIdx)
                        {
                                pathCount++;
                                continue;
                        }
                        var c = caveList[cs];
                        foreach(var s in c.FastNeighbors.Where(x => ((1 << x) & (v)) == 0))
                        {
                                var nv = v;
                                if(!caveList[s].Big)
                                {
                                        nv += (1 << s);
                                }
                                q.Enqueue((s, nv));
                        }
                }

                Console.WriteLine(pathCount);
                // Part 2
                var allPaths = new HashSet<string>();
                var allPathsBI = new HashSet<BigInteger>();
                foreach(var (i, repeatCave) in caveList.Enumerate())
                {
                        if(repeatCave.Big || i == startIdx || i == endIdx)
                        {
                                continue;
                        }

                        var p2q = new Queue<(int, (int count, BigInteger intPath), int, bool)>();
                        p2q.Enqueue((startIdx, (1, startIdx), (1 << startIdx), false));

                        while(p2q.Any())
                        {
                                var (cs, p, v, vr) = p2q.Dequeue();

                                if(cs == endIdx)
                                {
                                        //allPaths.Add(string.Join(',', p));
                                        allPathsBI.Add(p.intPath);
                                        continue;
                                }
                                var c = caveList[cs];
                                foreach (var s in c.FastNeighbors.Where(x => ((1 << x) & (v)) == 0))
                                {
                                        var nv = v;
                                        var newIntPath = p.intPath + ((BigInteger)s <<(p.count * 4));
                                        // Only add small caves
                                        if(!caveList[s].Big)
                                        {
                                                // if we've visited already and it is the repeat cave or if it's not the repeat cave 
                                                // add to the visited list
                                                if((vr && i == s) || s != i)
                                                {
                                                        nv += (1 << s);
                                                }
                                        }
                                        var visitedRepeat = vr;
                                        if(i == s)
                                        {
                                                visitedRepeat = true;
                                        }

                                        p2q.Enqueue((s, (p.count + 1, newIntPath), nv, visitedRepeat));
                                }
                        }
                }


                Console.WriteLine(allPathsBI.Count());
        }

        private class Cave
        {
                public bool Big;
                public HashSet<string> Neighbors = new HashSet<string>();
                public HashSet<int> FastNeighbors = new HashSet<int>();
                public int NeighborMask = 0;
        }
}