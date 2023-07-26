namespace AOC21;
public class Day9 : IDay
{
        int rows, cols;

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                rows = lines.Count();
                cols = lines.First().Count();
                var map = new int[rows,cols];
                foreach(var (r, line) in Enumerate(lines))
                {
                        foreach(var (c, h) in Enumerate(line))
                        {
                                map[r,c] = Convert.ToInt32(h) - '0';
                        }
                }

                var lows = new List<(int, int)>();
                var dirs = new [] { (-1, 0), (1, 0), (0, 1), (0, -1)};
                var p1Score = 0;
                for(var r = 0; r < rows; r++)
                {
                        for(var c = 0; c < cols; c++)
                        {
                                var h = map[r,c];
                                var isLow = true;
                                foreach(var (dr, dc) in dirs)
                                {
                                        var (cr, cc) = (r + dr, c + dc);
                                        if(cr >= 0 && cr < rows && cc >= 0 && cc < cols)
                                        {
                                                var ch = map[cr, cc];
                                                if(ch <= h)
                                                {
                                                        isLow = false;
                                                }
                                        }
                                }
                                if(isLow)
                                {
                                        p1Score += h + 1;
                                        lows.Add((r,c));
                                }
                        }
                }
                
                Console.WriteLine(p1Score);

                var basinSizes = lows.Select(x => GetBasinSize(x.Item1, x.Item2, map));
                var top3 = basinSizes.OrderByDescending(x => x).Take(3).ToArray();
                var p2 = top3[0] * top3[1] * top3[2];
                Console.WriteLine(p2);
        }

        // there are a bunch of ways to approach this but the easiest is probably to just search
        // key observation is that because each point can only be part of one basin and 9s are considered
        // a part of no basin then each basin must be bordered by 9s, so we just search neighbors until
        // we find 9s or edges
        private int GetBasinSize(int lr, int lc, int[,] map)
        {
                var dirs = new [] { (-1, 0), (1, 0), (0, 1), (0, -1)};
                var q = new Queue<(int, int)>();
                q.Enqueue((lr, lc));
                var visited = new HashSet<(int, int)>();
                var size = 0;
                while(q.Any())
                {
                        var (r, c) = q.Dequeue();
                        if(visited.Contains((r, c)))
                        {
                                continue;
                        }
                        foreach(var (dr, dc) in dirs)
                        {
                                var (cr, cc) = (r + dr, c + dc);
                                if(cr >= 0 && cr < rows && cc >= 0 && cc < cols)
                                {
                                        var ch = map[cr, cc];
                                        if(ch != 9)
                                        {
                                                q.Enqueue((cr, cc));
                                        }
                                }
                        }
                        visited.Add((r, c));
                        size++;
                }
                return size;
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