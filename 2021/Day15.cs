namespace AOC21;
public class Day15 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var map = lines.AsIntGrid();


                Console.WriteLine(GetMinPath(map));

                var w = map.GetLength(0) * 5;
                var h = map.GetLength(1) * 5;
                var expandendMap = new int[h, w];


                foreach(var ((r, c), v) in lines.IntGrid())
                {
                        foreach(var (i, j) in Helpers.GridIter(5, 5))
                        {
                                var newRisk = v + i + j;
                                newRisk = newRisk > 9 ? newRisk - 9 : newRisk;
                                expandendMap[r + (map.GetLength(0) * j), c + (map.GetLength(1) * i)] = newRisk;
                        }
                }

                Console.WriteLine(GetMinPath(expandendMap));
        }

        // Simply Djikstra algo
        private int GetMinPath(int[,] map)
        {
                var visited = new HashSet<(int, int)>();
                var pq = new PriorityQueue<((int, int), int), int>(map.GetLength(0) * map.GetLength(1));

                var ep = (map.GetLength(0) - 1, map.GetLength(1) - 1);
 
                pq.Enqueue(((0, 0), 0), 0);

                while(pq.Count > 0)
                {
                        var (p, s) = pq.Dequeue();
                        if(!visited.Add(p))
                        {
                                continue;
                        }
                        foreach(var (cr, cc) in p.cDirs().Where(x => map.ContainsPoint(x.cr, x.cc)))
                        {
                                if(p == ep)
                                {
                                        return s;
                                }
                                var ns = s + map[cr, cc];
                                pq.Enqueue(((cr, cc), ns), ns);
                        }
                }
                return int.MaxValue;
        }
}