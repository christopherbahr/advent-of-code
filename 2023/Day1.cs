using System.Numerics;

namespace AOC23;
public class Day1 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var p1 = 0;
                var p2 = 0;
                var digits = new [] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }.Enumerate();
                foreach(var line in lines)
                {
                        var p1Digits = new List<int>();
                        var p2Digits = new List<int>();
                        foreach(var (i, c) in line.Enumerate())
                        {
                                var (isdigit, val) = c.GetDigit();
                                if(isdigit)
                                {
                                        p1Digits.Add(val);
                                        p2Digits.Add(val);
                                }

                                foreach(var (j, d) in digits)
                                {
                                        if(line.IndexOf(d, i) == i)
                                        {
                                                p2Digits.Add(j + 1);
                                                break;
                                        }
                                }
                        }
                        p1 += int.Parse($"{p1Digits.First()}{p1Digits.Last()}");
                        p2 += int.Parse($"{p2Digits.First()}{p2Digits.Last()}");
                }
                Console.WriteLine((p1, p2));

        }
}