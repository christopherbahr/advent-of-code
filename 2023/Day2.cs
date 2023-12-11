
namespace AOC23;
public class Day2 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var p1 = 0;
                var p2 = 0;
                foreach (var (i, line) in lines.Enumerate())
                {
                        var possible = true;
                        var maxBlue = 0;
                        var maxGreen = 0;
                        var maxRed = 0;
                        // All we care about is the maximum revealed value for each cube
                        // The fact that it takes place iteratively doesn't matter. We can consider
                        // each number, color pair to be independent
                        foreach (var cube in line.Split(':')[1].Split(';').SelectMany(x => x.Split(',')))
                        {
                                var val = cube.GetInts().First();
                                if (cube.Contains("blue"))
                                {
                                        maxBlue = Math.Max(val, maxBlue);
                                        if (val > 14)
                                        {
                                                possible = false;
                                        }
                                }
                                else if (cube.Contains("green"))
                                {
                                        maxGreen = Math.Max(val, maxGreen);
                                        if (val > 13)
                                        {
                                                possible = false;
                                        }
                                }
                                else if (cube.Contains("red"))
                                {
                                        maxRed = Math.Max(val, maxRed);
                                        if (val > 12)
                                        {
                                                possible = false;
                                        }
                                }
                        }
                        p2 += maxBlue * maxGreen * maxRed;
                        if (possible)
                        {
                                p1 += i + 1;
                        }

                }
                Console.WriteLine((p1, p2));

        }
}