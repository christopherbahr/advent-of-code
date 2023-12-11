namespace AOC23;
public class Day10 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var (sr, sc) = (0, 0);
                var cg = lines.AsCharGrid();
                foreach(var ((r,c), v) in cg.Enumerate())
                {
                        if(v == 'S')
                        {
                                (sr, sc) = (r, c);
                        }
                }

                var connectionDict = new Dictionary<char, char[]>
                {
                        ['|'] = new []{ 'N', 'S'},
                        ['-'] = new []{ 'E', 'W'},
                        ['L'] = new[] { 'N', 'E' },
                        ['J'] = new[] { 'N', 'W' },
                        ['7'] = new[] { 'S', 'W' },
                        ['F'] = new[] { 'S', 'E' },
                        ['S'] = new[] { 'N', 'S', 'E', 'W' },
                };

                var connectionPairs = new List<((int, int), char, char)>
                {
                        (Helpers.North, 'N', 'S'),
                        (Helpers.South, 'S', 'N'),
                        (Helpers.East, 'E', 'W'),
                        (Helpers.West, 'W', 'E'),
                };

                var loopNodes = new List<(int, int)>();
                var q = new Queue<((int, int), int)>();
                q.Enqueue(((sr, sc), 0));
                var seen = new HashSet<(int, int)>();
                var maxDist = 0;
                while(q.Any())
                {
                        var ((r, c), d) = q.Dequeue();
                        if(!seen.Add((r,c)));
                        maxDist = Math.Max(maxDist, d);
                        foreach(var ((dr, dc), me, pair) in connectionPairs)
                        {
                                var (cr, cc) = (r + dr, c + dc);
                                if(!cg.ContainsPoint(cr, cc) || cg[cr, cc] == '.' || cg[cr, cc] == 'S' || !connectionDict[cg[r,c]].Contains(me))
                                {
                                        continue;
                                }

                                var n = cg[cr, cc];
                                if (connectionDict[n].Contains(pair) && !seen.Contains((cr, cc)))
                                {
                                        q.Enqueue(((cr, cc), d + 1));
                                }
                        }
                }
                var counter = 0;
                Console.WriteLine(maxDist);
                var ng = lines.AsCharGrid();
                foreach(var ((r,c), v) in cg.Enumerate())
                {
                        if(!seen.Contains((r, c)))
                        {
                                ng[r,c] = '.';
                                counter++;
                        }
                }

                // Plan:
                // If you imagine each pipe exists on a 3x3 grid each pipe segment takes up 3 of those cells
                // For exampte:
                //     XOX       XXX
                // | = XOX   7 = OOX
                //     XOX       XOX
                // If we expand the puzzle to be 3x it's normal size and replace each pipe segment with the corresponding
                // 3x3 segment we can easily do a reachability algorithm from the edge and find all the reachable points.
                // Then we just count the unreachable points that are not part of one of the pipe segments and divide by 9
                
                var map = new Dictionary<char, char[]>()
                {
                        ['|'] = new [] {
                                        '.', 'X', '.',
                                        '.', 'X', '.',
                                        '.', 'X', '.'
                                       },
                        ['-'] = new [] {
                                        '.', '.', '.',
                                        'X', 'X', 'X',
                                        '.', '.', '.'
                                       },
                        ['L'] = new [] {
                                        '.', 'X', '.',
                                        '.', 'X', 'X',
                                        '.', '.', '.'
                                       },
                        ['J'] = new [] {
                                        '.', 'X', '.',
                                        'X', 'X', '.',
                                        '.', '.', '.'
                                       },
                        ['7'] = new [] {
                                        '.', '.', '.',
                                        'X', 'X', '.',
                                        '.', 'X', '.'
                                       },
                        ['F'] = new [] {
                                        '.', '.', '.',
                                        '.', 'X', 'X',
                                        '.', 'X', '.'
                                       },
                        ['S'] = new [] { // by inspection rather than generalized
                                        '.', '.', '.',
                                        '.', 'X', 'X',
                                        '.', 'X', '.'
                                       },
                        ['.'] = new [] {
                                        '.', '.', '.',
                                        '.', '.', '.',
                                        '.', '.', '.'
                                       },
                };


                var nng = new char[cg.GetLength(0) * 3 + 1, cg.GetLength(1) * 3 + 1];
                // pad the edge of the map with '.' to make sure all of the outside is connected
                foreach(var i in Enumerable.Range(0,nng.GetLength(0)))
                {
                        var l = nng.GetLength(1) - 1;
                        nng[i,0] = '.';
                        nng[i,l] = '.';
                }
                foreach(var i in Enumerable.Range(0,nng.GetLength(1)))
                {
                        var l = nng.GetLength(0) - 1;
                        nng[0,i] = '.';
                        nng[l,i] = '.';
                }
                // Expand the map
                foreach(var ((r,c), v) in ng.Enumerate())
                {
                        var m = map[v];
                        var baseR = 1 + (3 * r);
                        var baseC = 1 + (3*c);
                        // Map the single char to the relevant 3x3 box
                        foreach(var wr in Enumerable.Range(0, 3))
                        {
                                foreach(var wc in Enumerable.Range(0,3))
                                {
                                       var idx = 3 * wr + wc;
                                       var wv = m[idx];
                                       nng[baseR + wr, baseC + wc] = wv;
                                }
                        }
                }

                // now find all the reachable points from the corner
                var qq = new Queue<(int, int)>();
                qq.Enqueue((0, 0));
                var seen2 = new HashSet<(int, int)>();
                while(qq.Any())
                {
                        var (r, c) = qq.Dequeue();
                        seen2.Add((r, c));
                        foreach(var (cr, cc) in (r,c).cDirs(nng))
                        {
                                if(nng[cr, cc] == '.' && !seen2.Contains((cr, cc)))
                                {
                                        seen2.Add((cr, cc));
                                        qq.Enqueue((cr, cc));
                                }
                        }
                }

                foreach(var (or, oc) in seen2)
                {
                        nng[or, oc] = 'O';
                }
                
                // Now iterate the original one again and check the corresponding location in the expanded one to see if the 
                // entire block is '.', if so add to the counter and I think that's all we need to know.
                var p2 = 0;
                foreach(var ((r,c), v) in ng.Enumerate())
                {
                        var m = map[v];
                        var baseR = 1 + (3 * r);
                        var baseC = 1 + (3*c);
                        // Map the single char to the relevant 3x3 box
                        var allDots = true;
                        foreach(var wr in Enumerable.Range(0, 3))
                        {
                                foreach(var wc in Enumerable.Range(0,3))
                                {
                                       var idx = 3 * wr + wc;
                                       //var wv = m[idx];
                                       var thing = nng[baseR + wr, baseC + wc];
                                       if(thing != '.')
                                       {
                                                allDots = false;
                                                break;
                                       }
                                }
                        }
                        if (allDots)
                        {
                                ng[r, c] = 'I';
                                p2++;
                        }
                }
                Console.WriteLine(p2);
        }
}