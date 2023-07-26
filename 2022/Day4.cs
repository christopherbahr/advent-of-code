// Camp Cleanup
namespace AOC22;
public class Day4 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var completeOverlapCounter = 0;
                var partialOverlapCounter = 0;
                foreach(var line in lines)
                {
                        var assignments = line.Split(',');
                        var firstRange = assignments[0].Split('-').Select(int.Parse);
                        var secondRange = assignments[1].Split('-').Select(int.Parse);

                        // detect complete overlap
                        if((firstRange.First() <= secondRange.First() && firstRange.Last() >= secondRange.Last())
                        || (secondRange.First() <= firstRange.First() && secondRange.Last() >= firstRange.Last()))
                        {
                                completeOverlapCounter++;
                        }

                        if((firstRange.First() <= secondRange.First() && firstRange.Last() >= secondRange.First())
                        ||(secondRange.First() <= firstRange.First() && secondRange.Last() >= firstRange.First()) )
                        {
                                partialOverlapCounter++;
                        }
                }
                
                Console.WriteLine((completeOverlapCounter.ToString(), partialOverlapCounter.ToString()));
        }
}