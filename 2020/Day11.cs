namespace AOC20;
public class Day11 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var cg = lines.AsCharGrid();
                var changed = true;
                var toClear = new HashSet<(int, int)>();
                var toOccupy = new HashSet<(int, int)>();
                while (changed)
                {
                        changed = false;
                        foreach(var ((r, c), v) in cg.Enumerate())
                        {
                                if(v == '.')
                                {
                                        continue;
                                }
                                var occCounter = 0;
                                foreach (var (cr, cc) in (r,c).allDirs().Where(x => cg.ContainsPoint(x.cr, x.cc)))
                                {
                                        if (cg[cr, cc] == '#')
                                        {
                                                occCounter++;
                                        }
                                }
                                if (v == '#' && occCounter >= 4)
                                {
                                        toClear.Add((r, c));
                                }
                                else if (v == 'L' && occCounter == 0)
                                {
                                        toOccupy.Add((r, c));
                                }
                        }
                        foreach(var (r, c) in toClear)
                        {
                                cg[r,c] = 'L';
                        }
                        foreach(var (r, c) in toOccupy)
                        {
                                cg[r,c] = '#';
                        }
                        if(toClear.Any() || toOccupy.Any())
                        {
                                changed = true;
                        }
                        toClear.Clear();
                        toOccupy.Clear();
                }
                var p1 = 0;
                foreach(var (_, v) in cg.Enumerate())
                {
                        if(v == '#')
                        {
                                p1++;
                        }
                }
                Console.WriteLine(p1);

                cg = lines.AsCharGrid();
                changed = true;
                while(changed)
                {
                        changed = false;
                        foreach(var ((r, c), v) in cg.Enumerate())
                        {
                                if(v == '.')
                                {
                                        continue;
                                }
                                var occCounter = 0;
                                // It seems ilke there should be some optimizations here
                                // Like if you're over 5 then just stop
                                // if you're > 0 but can't possibly make it to 5
                                // then stop
                                // unfortunately the calculation is so cheap that adding
                                // the check actually slows things down
                                foreach(var (dr, dc) in Helpers.rawAllDirs)
                                {
                                        var (cr, cc) = (r + dr, c + dc);
                                        while (cg.ContainsPoint(cr, cc))
                                        {
                                                if(cg[cr, cc] == '#')
                                                {
                                                        occCounter++;
                                                        break;
                                                }
                                                else if(cg[cr, cc] == 'L')
                                                {
                                                        break;
                                                }
                                                (cr, cc) = (cr + dr, cc + dc);
                                        }
                                }
                                if (v == '#' && occCounter >= 5)
                                {
                                        toClear.Add((r, c));
                                }
                                else if (v == 'L' && occCounter == 0)
                                {
                                        toOccupy.Add((r, c));
                                }
                        }
                        foreach(var (r, c) in toClear)
                        {
                                cg[r,c] = 'L';
                        }
                        foreach(var (r, c) in toOccupy)
                        {
                                cg[r,c] = '#';
                        }
                        if(toClear.Any() || toOccupy.Any())
                        {
                                changed = true;
                        }
                        toClear.Clear();
                        toOccupy.Clear();
                }
                var p2 = 0;
                foreach(var (_, v) in cg.Enumerate())
                {
                        if(v == '#')
                        {
                                p2++;
                        }
                }
                Console.WriteLine(p2);
        }
}