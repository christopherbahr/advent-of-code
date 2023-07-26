namespace AOC21;
public class Day17 : IDay
{
        int xmin, xmax, ymin, ymax;
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var line = lines.Skip(0).First();
                var ranges = line.GetInts();
                xmin = ranges[0];
                xmax = ranges[1];
                ymin = ranges[2];
                ymax = ranges[3];

                // We could use some math to reduce the search space and avoid
                // actually simulating every step
                // The space isn't actually that big though so don't bother
                // The only trick we need is to realize that x and y evolve indepepdently
                // so find all the initial dx that can end in the target zone and all the 
                // initial dy that can end in the target zone and just try all combinations of the two


                var xCandidates = new List<int>();
                var yCandidates = new List<int>();

                for(var dx = 1; dx <= xmax+1; dx++)
                {
                        // Make sure it at least gets there
                        var xDist = ((dx * dx) + dx) / 2;
                        if(xDist < xmin)
                        {
                                continue;
                        }
                        if (DoesXHit(dx))
                        {
                                xCandidates.Add(dx);
                        }
                }

                for(var dy = ymin-1; dy <= Math.Abs(ymin)+1; dy++)
                {
                        // Make sure it at least gets there
                        if (DoesYHit(dy))
                        {
                                yCandidates.Add(dy);
                        }
                }

                var highestY = 0;
                var count = 0;
                foreach(var dx in xCandidates)
                foreach (var dy in yCandidates)
                {
                        var (h, hy) = SimulateTrajectory(dx, dy);
                        if(h)
                        {
                                count++;
                                highestY = Math.Max(hy, highestY);
                        }
                }
                Console.WriteLine(highestY);
                Console.WriteLine(count);

        }

        private bool DoesYHit(int dy)
        {
                var y = 0;
                while(y >= ymin)
                {
                        y += dy;
                        dy -= 1;
                        if(y >= ymin && y <= ymax)
                        {
                                return true;
                        }
                }
                return false;
        }

        private bool DoesXHit(int dx)
        {
                var x = 0;
                while(x<= xmax)
                {
                        x += dx;
                        if(dx > 0)
                        {
                                dx -= 1;
                        }
                        if(x >= xmin && x <= xmax)
                        {
                                return true;
                        }
                }
                return false;
        }

        private (bool hit, int hy) SimulateTrajectory(int dx, int dy)
        {
                var (x, y) = (0, 0);
                var highestY = 0;
                while(x<= xmax && y >= ymin)
                {
                        x += dx;
                        y += dy;
                        highestY = Math.Max(y, highestY);
                        dy -= 1;
                        if(dx > 0)
                        {
                                dx -= 1;
                        }
                        if(x >= xmin && x <= xmax && y >= ymin && y <= ymax)
                        {
                                return (true, highestY);
                        }
                }
                return (false, 0);
        }

}