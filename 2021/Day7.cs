namespace AOC21;
public class Day7 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                // It's not immediately obvious to me how best to do this
                // The easy option is to just simulate all positions
                // There aren't that many possibilities because we're definitely
                // going to want to be between the smallest and largest crab position

                var positions = lines.First().Split(',').Select(int.Parse);
                var min = positions.Min();
                var max = positions.Max();
                var count = positions.Count();

                long minDist = int.MaxValue;
                for(var i = min; i <= max; i++)
                {
                        var totalDist = 0;
                        foreach(var pos in positions)
                        {
                                totalDist += Math.Abs(pos - i);
                        }
                        if(totalDist < minDist)
                        {
                                minDist = totalDist;
                        }
                }
                Console.WriteLine(minDist);

                minDist = int.MaxValue;
                // Now part 2, just changes the cost.
                for(var i = min; i <= max; i++)
                {
                        var totalDist = 0;
                        foreach(var pos in positions)
                        {
                                var linDist = Math.Abs(pos - i);
                                var triangleDist = (linDist + (linDist * linDist)) /2;
                                totalDist += triangleDist;
                        }
                        if(totalDist < minDist)
                        {
                                minDist = totalDist;
                        }
                }
                Console.WriteLine(minDist);

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