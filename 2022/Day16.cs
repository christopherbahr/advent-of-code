namespace AOC22;
public class Day16 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        private static Dictionary<int, GraphNode> GraphMap = new Dictionary<int, GraphNode>();
        public void Execute()
        {

                var lines = File.ReadAllLines(inputFile);

                var lookup = new Dictionary<string, int>();

                var idx = 0;
                for(var i = 0; i < lines.Length; i++)
                {
                        var input = lines[i].Split();
                        var rate = int.Parse(input[4].Split('=')[1].TrimEnd(';'));
                        // give the useful ones low numbers so they pack better into a bitfield
                        if(rate > 0)
                        {
                                lookup[input[1]] = idx;
                                idx++;
                        }
                        else
                        {
                                lookup[input[1]] = i + 100;
                        }
                }

                foreach(var line in lines)
                {
                        var input = line.Split();
                        var gn = new GraphNode();
                        var myNum = lookup[input[1]];
                        GraphMap[myNum] = gn;

                        var rate = input[4].Split('=')[1].TrimEnd(';');
                        gn.Value = int.Parse(rate);
                        gn.Id = myNum;

                        for(int i = 9; i < input.Length; i++)
                        {
                                var tn = input[i].TrimEnd(',');
                                gn.Edges.Add(lookup[tn]);
                        }
                }

                var sn = GraphMap[lookup["AA"]];

                foreach(var (_, gn) in GraphMap)
                {
                        PopulatePaths(gn);
                }


                var viablePaths = PopulateViablePaths(sn, 30, new List<int>());

                var bestScore = 0;
                foreach(var path in viablePaths)
                {
                        var score = ScorePath(path, 30);
                        bestScore = Math.Max(score, bestScore);
                }

                viablePaths = PopulateViablePaths(sn, 26, new List<int>());

                // There are probably some tricks to reduce the time it takes
                // to score all these but scoring is actually pretty fast so 
                // I won't bother.

                var p2List = new List<(List<int> path, int score, int bitmask)>();
                foreach(var path in viablePaths)
                {
                        var bitMask = 0;
                        foreach(var entry in path.Skip(1))
                        {
                                bitMask += 1 << entry;
                        }
                        var score = ScorePath(path, 26);
                        if(score < 900)
                        {
                                continue;
                        }
                        p2List.Add((path, score, bitMask));
                }


                // Next up is to store the above scores in a list with (path, score, bitmask)
                // then n^2 iterate the list, compare the bitmasks, if l1 & l2 = 0 then sum their scores
                // if not then 

                var bestP2Score = 0;

                p2List = p2List.OrderByDescending(x => x.score).ToList();

                for(var i = 0; i < p2List.Count; i++)
                {
                        for(var j = i + 1; j < p2List.Count; j++)
                        {
                                var myPath = p2List[i];
                                var ePath = p2List[j];
                                if((ePath.bitmask & myPath.bitmask) != 0)
                                {
                                        continue;
                                }
                                bestP2Score = Math.Max(myPath.score + ePath.score, bestP2Score);
                        }
                }

                Console.WriteLine( (bestScore.ToString(), bestP2Score.ToString()));
        }


        private List<List<int>> PopulateViablePaths(GraphNode startNode, int time, List<int> pathSoFar)
        {
                var paths = new List<List<int>>();

                // this is my pase path
                var myPath = pathSoFar.Append(startNode.Id).ToList();

                var addedSome = false;
                // don't go places we've already gone
                foreach(var (id, cost) in startNode.UsefulPaths.Where(x => !pathSoFar.Contains(x.Key)))
                {
                        // add 1 to acount for turning on the valve
                        if(time - cost - 1 > 0)
                        {
                                var pathsIncludingThisNode = PopulateViablePaths(GraphMap[id], time - cost - 1, myPath);
                                if(pathsIncludingThisNode.Any())
                                {
                                        addedSome = true;
                                }
                                paths.AddRange(pathsIncludingThisNode);
                        }
                }

                if (!addedSome)
                {
                        paths.Add(myPath);
                }

                return paths;
        }

        private int ScorePath(List<int> path, int time)
        {
                var score = 0;
                GraphNode cn;
                var timeLeft = time;

                for(var i = 0; i < path.Count; i++)
                {
                        cn = GraphMap[path[i]];
                        score += cn.Value * timeLeft;
                        if(i != path.Count - 1)
                        {
                                // subtract the time it will take to get to the next path
                                timeLeft -= cn.UsefulPaths[path[i + 1]] + 1;
                        }
                }

                return score;
        }
        private void PopulatePaths(GraphNode gn)
        {
                var q = new  Queue<(GraphNode, int)>();
                q.Enqueue((gn, 0));
                var pathVisited = new HashSet<int>();
                while(q.Any())
                {
                        var (cn, dist) = q.Dequeue();
                        foreach(var nn in cn.Edges.Where(x => !pathVisited.Contains(x)))
                        {
                                q.Enqueue((GraphMap[nn], dist+ 1));
                        }
                        pathVisited.Add(cn.Id);

                        if(cn.Value > 0 && cn.Id != gn.Id)
                        {
                                gn.UsefulPaths[cn.Id] = dist;
                        }
                }
        }

        private class GraphNode
        {
                public int Id {get; set;}
                public int Value {get; set;}
                public List<int> Edges = new List<int>();
                // GraphNodes & how far away they are
                public Dictionary<int, int> UsefulPaths = new Dictionary<int, int>();
        }
}