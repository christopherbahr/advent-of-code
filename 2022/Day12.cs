
namespace AOC22;
public class Day12 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        private class GraphNode
        {
                public int Value {get; set;}
                public List<(int, int)> Edges = new List<(int, int)>();
                public int Distance {get; set;} = 9999;
                public bool IsEnd {get; set;}
                public (int, int) Position {get; set;}
                public long Id {get; set;}
        }


        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var heightMap = new int[lines.Count(), lines.First().Count()];
                var graph = new GraphNode[lines.Count(), lines.First().Count()];
                
                GraphNode endNode = null;

                var dirs = new[] { (0, 1), (0, -1), (1, 0), (-1, 0)};

                var rows = lines.Length;
                var cols = lines[0].Length;

                var p1 = 0;
                var p2 = 0;
                var isP1 = true;
                for (var r = 0; r < rows; r++)
                {
                        for (var c = 0; c < cols; c++)
                        {
                                var val = (int)lines[r][c];
                                var isStart = val == 'S';
                                var isEnd = val == 'E';
                                var gn = new GraphNode();
                                gn.Id = r + (c << 32);
                                if (isStart)
                                {
                                        val = (int)'a';
                                        gn.Distance = 0;
                                }
                                else if (isEnd)
                                {
                                        val = (int)'z';
                                        endNode = gn;
                                }

                                gn.Value = val;
                                gn.Position = (r, c);
                                foreach (var (dr, dc) in dirs)
                                {
                                        var cr = r + dr;
                                        var cd = c + dc;
                                        if (0 <= cr && cr < rows && 0 <= cd && cd < cols)
                                        {
                                                var height = lines[cr][cd];
                                                height = height == 'E' ? 'z' : height == 'S' ? 'a' : height;
                                                if (height - val <= 1)
                                                {
                                                        gn.Edges.Add((cr, cd));
                                                }
                                        }
                                }
                                graph[r, c] = gn;
                        }
                }
                while(true)
                {
                        if(!isP1)
                        {
                                foreach(var gn in graph)
                                {
                                        if(gn.Value == 'a')
                                        {
                                                gn.Distance = 0;
                                        }
                                        else
                                        {
                                                gn.Distance = int.MaxValue;
                                        }
                                }
                        }
                        var unvisited = new List<GraphNode>();

                        foreach (var gn in graph)
                        {
                                unvisited.Add(gn);
                        }

                        while (unvisited.Any())
                        {
                                var cn = unvisited.MinBy(x => x.Distance);
                                unvisited.Remove(cn);

                                foreach (var edge in cn.Edges)
                                {
                                        var tn = graph[edge.Item1, edge.Item2];
                                        if (tn.Distance > cn.Distance + 1)
                                        {
                                                tn.Distance = cn.Distance + 1;
                                        }
                                }
                        }
                        if(isP1)
                        {
                                p1 = endNode.Distance;
                                isP1 = false;
                        }
                        else
                        {
                                p2 = endNode.Distance;
                                break;
                        }
                }


                Console.WriteLine( (p1.ToString(), p2.ToString()));
        }

        private class DJComparer : IComparer<int>
        {
                public int Compare(int x, int y)
                {
                        return x - y;
                }
        }

        private List<int> ParseInts(string s)
        {
                var components = s.Split(' ');
                var retList = new List<int>();
                foreach(var component in components)
                {
                        if(int.TryParse(component, out var num))
                        {
                                retList.Add(num);
                        }

                }
                return retList;
        }
}