namespace AOC22;
public class Day14 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var state = new Status[700, 700];
                var highestRow = 0;
                var closestRock = int.MaxValue;
                foreach(var line in lines)
                {
                        var pairs = line.Split(" -> ").Select(x=> x.Split(',')).Select(x => (int.Parse(x[0]), int.Parse(x[1])));

                        var (cc, cr) = pairs.First();
                        state[cc, cr] = Status.Rock;
                        if(cc == 500)
                        {
                                closestRock = Math.Min(closestRock, cr);
                        }
                        if(cr > highestRow)
                        {
                                highestRow = cr;
                        }
                        foreach(var (c, r) in pairs.Skip(1))
                        {
                                if (r > highestRow)
                                {
                                        highestRow = cr;
                                }
                                var dr = Math.Sign(r - cr);
                                var dc = Math.Sign(c - cc);
                                while (cr != r || cc != c)
                                {
                                        cr += dr;
                                        cc += dc;
                                        state[cc, cr] = Status.Rock;
                                        if (cc == 500)
                                        {
                                                closestRock = Math.Min(closestRock, cr);
                                        }
                                }
                        }
                }

                for (int c = 0; c < 700; c++)
                {
                        state[c, highestRow + 2] = Status.Rock;
                }

                // now simulate sand

                var closest = closestRock;

                var full = false;
                var counter = 0;
                var p1Counter = 0;
                var countp1 = true;
                while(!full)
                {
                        var (cc, cr) = (500, closest - 1);
                        var landed = false;
                        while(!landed)
                        {
                                if(state[cc, cr + 1] == Status.Air)
                                {
                                        cr = cr+1;
                                }
                                else if(state[cc - 1, cr + 1] == Status.Air)
                                {
                                        cc = cc - 1;
                                        cr = cr + 1;
                                }
                                else if(state[cc + 1, cr + 1] == Status.Air)
                                {
                                        cc = cc + 1;
                                        cr = cr + 1;
                                }
                                else
                                {
                                        state[cc, cr] = Status.Sand;
                                        if(cc == 500)
                                        {
                                                closest = Math.Min(closest, cr);
                                        }
                                        landed = true;
                                        if(cr == highestRow+1)
                                        {
                                                countp1 = false;
                                        }
                                        if(countp1)
                                        {
                                                p1Counter++;
                                        }
                                        if(cc == 500 && cr == 0)
                                        {
                                                full = true;
                                        }
                                        counter++;
                                }
                        }
                }

                Console.WriteLine( (p1Counter.ToString(), counter.ToString()));
        }

        public enum Status{
                Air = 0, 
                Sand,
                Rock
        }
}