namespace AOC21;
public class Day2 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var (x, d) = (0, 0);
                var (x2, d2, a) = (0, 0, 0);

                var p1 = 0;
                var p2 = 0;
                foreach(var (i, line) in Enumerate(lines))
                {
                        var comps = line.Split(' ');
                        var (dir, dist) = (comps[0], int.Parse(comps[1]));

                        var (dx, dd) = (0, 0);
                        var (dx2, dd2, da) = (0, 0, 0);

                        switch (dir)
                        {
                                case "forward":
                                        dx = 1;
                                        dd2 = a;
                                        break;
                                case "down":
                                        dd = 1;
                                        break;
                                case "up":
                                        dd = -1;
                                        break;
                        }

                        a = a + dist * dd;
                        x = x + dist * dx;
                        d = d + dist * dd;

                        d2 = d2 + dist * dd2;
                }
                p1 = x * d;
                p2 = x * d2;
                Console.WriteLine(p1);
                Console.WriteLine(p2);

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