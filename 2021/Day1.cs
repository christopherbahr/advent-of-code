namespace AOC21;
public class Day1 : IDay
{

        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var last = -1;
                var last3 = new List<int>();
                var incrCounter = 0;
                var p2 = 0;
                foreach (var (i, line) in lines.Enumerate())
                {
                        var val = int.Parse(line);
                        if (val > last && last != -1)
                        {
                                incrCounter++;
                        }
                        last = val;

                        if (last3.Count < 3)
                        {
                                Console.WriteLine(i);
                                last3.Add(val);
                                continue;
                        }
                        else
                        {
                                var last3Sum = last3.Sum();
                                Console.WriteLine(last3Sum);
                                last3.RemoveAt(0);
                                last3.Add(val);
                                var newSum = last3.Sum();
                                if (newSum > last3Sum)
                                {
                                        p2++;
                                }

                        }
                }
                Console.WriteLine(incrCounter);
                Console.WriteLine(p2);

        }
}