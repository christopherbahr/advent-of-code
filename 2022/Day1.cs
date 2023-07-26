// Calorie Counting for Snacking Elves
namespace AOC22;
public class Day1 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var currentCalories = 0;

                var maxSoFar = 0;

                var top3 = new List<int>() { 0, 0, 0 };
                var minTop3 = 0;

                foreach (var line in lines)
                {
                        if (line == string.Empty)
                        {
                                if (currentCalories > minTop3)
                                {
                                        top3.Remove(minTop3);
                                        top3.Add(currentCalories);
                                        minTop3 = top3.Min();
                                }
                                currentCalories = 0;
                        }
                        else
                        {
                                currentCalories += int.Parse(line);
                        }
                }

                if (currentCalories > minTop3)
                {
                        maxSoFar = currentCalories;
                        top3.Remove(minTop3);
                        top3.Add(currentCalories);
                        minTop3 = top3.Min();
                }

                maxSoFar = top3.Max();
                var topThreeSum = top3.Sum();

                Console.WriteLine((maxSoFar.ToString(), topThreeSum.ToString()));
        }
}
