namespace AOC20;
public class Day12 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var commands = lines.Select(x => (x[0], int.Parse(x[1..])));
                var n = 0;
                var e = 0;

                var dn = 0;
                var de = 1;

                foreach(var (act, amt) in commands)
                {
                        switch(act)
                        {
                                case 'N':
                                        n += amt;
                                        break;
                                case 'S':
                                        n -= amt;
                                        break;
                                case 'E':
                                        e += amt;
                                        break;
                                case 'W':
                                        e -= amt;
                                        break;
                                case 'F':

                                        n += dn * amt;
                                        e += de * amt;
                                        break;
                                case 'L':
                                        var lt = amt / 90;
                                        foreach(var _ in Enumerable.Range(0,lt))
                                        {
                                                var t = dn;
                                                dn = de;
                                                de = -t;
                                        }
                                        break;
                                case 'R':
                                        var rt = amt / 90;
                                        foreach(var _ in Enumerable.Range(0, rt))
                                        {
                                                var t = dn;
                                                dn = -de;
                                                de = t;
                                        }
                                        break;
                        }
                }

                Console.WriteLine((0,0).ManhattanDistance((n, e)));

                var (sn, se) = (0, 0);
                var (wn, we) = (1, 10);

                foreach(var (act, amt) in commands)
                {
                        switch(act)
                        {
                                case 'N':
                                        wn += amt;
                                        break;
                                case 'S':
                                        wn -= amt;
                                        break;
                                case 'E':
                                        we += amt;
                                        break;
                                case 'W':
                                        we -= amt;
                                        break;
                                case 'F':
                                        sn += wn * amt;
                                        se += we * amt;
                                        break;
                                case 'L':
                                        var lt = amt / 90;
                                        foreach(var _ in Enumerable.Range(0,lt))
                                        {
                                                var t = wn;
                                                wn = we;
                                                we = -t;
                                        }
                                        break;
                                case 'R':
                                        var rt = amt / 90;
                                        foreach(var _ in Enumerable.Range(0, rt))
                                        {
                                                var t = wn;
                                                wn = -we;
                                                we = t;
                                        }
                                        break;
                        }
                }

                Console.WriteLine((0,0).ManhattanDistance((sn, se)));
        }
}