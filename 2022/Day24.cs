namespace AOC22;
public class Day24 : IDay
{
        // Note: My original implementation was DP, I think BFS is probably better
        // DP is more a DFS _and_ we're allowed to revisit squares so the only way to terminate is to put
        // an iteration limit on it
        // actually BFS has that problem too.
        // Anyway the internet says one good trick is to do BFS and have a "visited" set which is just places
        // we could be at the current time rather than a visited set with a time dimension
        // before we finally find something like an upper bound
        // this means hardcoding an 
        public string inputFile { get; set; } = "wip.in";
        List<(int, int, int)> rMap = new List<(int, int, int)>();
        List<(int, int, int)> cMap = new List<(int, int, int)>();
        int loopCount = 0;

        (sbyte, sbyte)[] dirs = new(sbyte, sbyte)[] { (1, 0), (0, 1), (0,0), (-1, 0), (0, -1) };
        Dictionary<(int, int, int), int> cache = new Dictionary<(int, int, int), int>();

        int height;
        int width;
        
        sbyte er, ec, sr, sc;

        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                height = lines.Count();
                width = lines[0].Count();


                (sr, sc) = (0, 0);
                (er, ec) = (0, 0);

                for (sbyte i = 0; i < height; i++)
                {
                        for (sbyte j = 0; j < width; j++)
                        {
                                if (i == 0 && lines[i][j] == '.')
                                {
                                        (sr, sc) = (i, j);
                                }
                                if (i == lines.Count() - 1 && lines[i][j] == '.')
                                {
                                        (er, ec) = (i, j);
                                }

                                var c = lines[i][j];
                                switch (c)
                                {
                                        case '>':
                                                cMap.Add((i, j, 1));
                                                break;
                                        case '<':
                                                cMap.Add((i, j, -1));
                                                break;
                                        case 'v':
                                                rMap.Add((i, j, 1));
                                                break;
                                        case '^':
                                                rMap.Add((i, j, -1));
                                                break;
                                }
                        }
                }

                PopulateStepMaps(height, width, rMap, cMap);


                var moves = BfsMovesToTarget(sr, sc, er, ec, 1);

                var retMoves = BfsMovesToTarget(er, ec, sr, sc, moves + 1);

                var allMoves = BfsMovesToTarget(sr, sc, er, ec, retMoves + 1);

                Console.WriteLine( (moves.ToString(), allMoves.ToString()));
        }

        int BfsMovesToTarget(sbyte r, sbyte c, sbyte tr, sbyte tc, int timestep)
        {
                var bfsQueue = new Queue<(sbyte cr, sbyte cc, int ct)>();

                var visited = new Dictionary<int, HashSet<(sbyte vr, sbyte vc)>>();

                var minSteps = int.MaxValue;

                bfsQueue.Enqueue((r, c, timestep));
                while(bfsQueue.Any())
                {
                        var (cr, cc, ct) = bfsQueue.Dequeue();
                        if(!visited.ContainsKey(ct))
                        {
                                visited[ct] = new HashSet<(sbyte vr, sbyte vc)>();
                        }

                        if(visited[ct].Contains((cr, cc)))
                        {
                                continue;
                        }
                        visited[ct].Add((cr, cc));

                        var rStepMap = GetRStepMap(ct);
                        var cStepMap = GetCStepMap(ct);
                        foreach (var (dr, dc) in dirs)
                        {
                                if ((cr + dr, cc + dc) == (tr, tc))
                                {
                                        minSteps = Math.Min(ct, minSteps);
                                        return minSteps;
                                }
                                else
                                {
                                        // We can't step out of bounds but we can step into the start position again
                                        if (!rStepMap.Contains(((sbyte)(cr + dr), (sbyte)(cc + dc))) && !cStepMap.Contains(((sbyte)(cr + dr), (sbyte)(cc + dc))))
                                        {
                                                if ((cr + dr >= height - 1 || cr + dr <= 0 || cc + dc >= width - 1 || cc + dc <= 0) && (cr + dr, cc + dc) != (r, c))
                                                {
                                                        continue;
                                                }
                                                var entry = ((sbyte)(cr + dr), (sbyte)(cc + dc), ct + 1);
                                                bfsQueue.Enqueue(entry);
                                        }
                                }
                        }
                }
                return minSteps;
        }

        Dictionary<int, HashSet<(sbyte, sbyte)>> rStepMapSet = new Dictionary<int, HashSet<(sbyte, sbyte)>>();
        Dictionary<int, HashSet<(sbyte, sbyte)>> cStepMapSet = new Dictionary<int, HashSet<(sbyte, sbyte)>>();
        void PopulateStepMaps(int rows, int cols, List<(int, int, int)> rMap, List<(int, int, int)> cMap)
        {
                for(int i = 0; i < rows - 2; i++)
                {
                        var rStepMap = new HashSet<(sbyte, sbyte)>();
                        foreach(var (r, c, dr) in rMap)
                        {
                                var localRow = r - 1;
                                var targetLocalRow = (localRow + i * dr) % (rows - 2);
                                if(targetLocalRow < 0)
                                {
                                        targetLocalRow += rows - 2;
                                }
                                var targetRow = targetLocalRow + 1;
                                rStepMap.Add(((sbyte)(targetRow), (sbyte)c));
                        }
                        rStepMapSet[i] = rStepMap;
                }

                for(int i = 0; i < cols - 2; i++)
                {
                        var cStepMap = new HashSet<(sbyte, sbyte)>();
                        foreach(var (r, c, dc) in cMap)
                        {
                                var localCol = c - 1;
                                var targetLocalCol = (localCol + i * dc) % (cols - 2);
                                if(targetLocalCol < 0)
                                {
                                        targetLocalCol += cols - 2;
                                }
                                var targetCol = targetLocalCol + 1;
                                cStepMap.Add(((sbyte)r, (sbyte)(targetCol)));
                        }
                        cStepMapSet[i] = cStepMap;
                }
        }

        HashSet<(sbyte,sbyte)> GetRStepMap(int timestep)
        {
                return rStepMapSet[timestep % (height - 2)];
        }

        HashSet<(sbyte,sbyte)> GetCStepMap(int timestep)
        {
                return cStepMapSet[timestep % (width - 2)];
        }

        void PrettyPrint(HashSet<(sbyte r, sbyte c)> rMap, HashSet<(sbyte r, sbyte c)> cMap)
        {
                for (sbyte i = 0; i < height; i++)
                {
                        var line = "";
                        for (sbyte j = 0; j < width; j++)
                        {
                                var occupied = rMap.Contains((i, j)) || cMap.Contains((i,j));
                                if ((sr, sc) == (i, j))
                                {
                                        line += 'E';
                                }
                                else if ((er, ec) == (i, j))
                                {
                                        line += 'T';
                                }
                                else if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                                {
                                        line += '#';
                                }
                                else if (occupied)
                                {
                                        line += "+";
                                }
                                else
                                {
                                        line += '.';
                                }
                        }
                        Console.WriteLine(line);
                }

        }
}