using System.Numerics;
namespace AOC22;
public class Day11 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);


                var lcm = 9699690;

                long p1 = 0;
                long p2 = 0;
                var isP1 = true;
                while(true)
                {
                        var turnCounter = 0;
                        var monkeys = InitializeMonkeys();
                        var turnThresh = isP1 ? 20 : 10000;
                        long score = 0;
                        while (true)
                        {
                                foreach (var monkey in monkeys)
                                {
                                        if (!monkey.Items.Any())
                                        {
                                                continue;
                                        }

                                        for (int i = 0; i < monkey.Items.Count; i++)
                                        {
                                                monkey.InspectionCount++;
                                                // inspect & cooldown
                                                var fullVal = monkey.Operation(monkey.Items[i]);
                                                long val = isP1 ? fullVal / 3 : fullVal % lcm;
                                                //Console.WriteLine($"New Value: {val}");
                                                if (monkey.Test(val))
                                                {
                                                        monkey.TrueMonkey.Items.Add(val);
                                                }
                                                else
                                                {
                                                        monkey.FalseMonkey.Items.Add(val);
                                                }
                                        }

                                        // all items thrown
                                        monkey.Items.Clear();
                                }
                                turnCounter++;
                                if (turnCounter == turnThresh)
                                {
                                        var p2TopTwo = monkeys.OrderBy(x => x.InspectionCount).Reverse().Take(2).ToList();
                                        score = p2TopTwo[0].InspectionCount * p2TopTwo[1].InspectionCount;
                                        break;
                                }
                        }
                        if (isP1)
                        {
                                p1 = score;
                                isP1 = false;
                        }
                        else
                        {
                                p2 = score;
                                break;
                        }

                }

                Console.WriteLine( (p1.ToString(), p2.ToString()));
        }

        private Monkey[] InitializeMonkeys()
        {
                var mo0 = new Monkey();
                var mo1 = new Monkey();
                var mo2 = new Monkey();
                var mo3 = new Monkey();
                var mo4 = new Monkey();
                var mo5 = new Monkey();
                var mo6 = new Monkey();
                var mo7 = new Monkey();

                mo0.Items = new long[] { 52, 60, 85, 69, 75, 75}.ToList();
                mo0.Operation = (x) => x * 17;
                mo0.Test = (x) => x % 13 == 0;
                mo0.TrueMonkey = mo6;
                mo0.FalseMonkey = mo7;

                mo1.Items = new long[] {96, 82, 61, 99, 82, 84, 85}.ToList();
                mo1.Operation = (x) => x + 8;
                mo1.Test = (x) => x % 7 == 0;
                mo1.TrueMonkey = mo0;
                mo1.FalseMonkey = mo7;

                mo2.Items = new long[] {95, 79}.ToList();
                mo2.Operation = (x) => x + 6;
                mo2.Test = (x) => x % 19 == 0;
                mo2.TrueMonkey = mo5;
                mo2.FalseMonkey = mo3;

                mo3.Items = new long[] {88, 50, 82, 65, 77}.ToList();
                mo3.Operation = (x) => x * 19;
                mo3.Test = (x) => x % 2 == 0;
                mo3.TrueMonkey = mo4;
                mo3.FalseMonkey = mo1;

                mo4.Items = new long[] {66, 90, 59, 90, 87, 63, 53, 88}.ToList();
                mo4.Operation = (x) => x + 7;
                mo4.Test = (x) => x % 5 == 0;
                mo4.TrueMonkey = mo1;
                mo4.FalseMonkey = mo0;

                mo5.Items = new long[] {92, 75, 62}.ToList();
                mo5.Operation = (x) => x * x;
                mo5.Test = (x) => x % 3 == 0;
                mo5.TrueMonkey = mo3;
                mo5.FalseMonkey = mo4;

                mo6.Items = new long[] {94, 86, 76, 67}.ToList();
                mo6.Operation = (x) => x + 1;
                mo6.Test = (x) => x % 11 == 0;
                mo6.TrueMonkey = mo5;
                mo6.FalseMonkey = mo2;

                mo7.Items = new long[] {57}.ToList();
                mo7.Operation = (x) => x + 2;
                mo7.Test = (x) => x % 17 == 0;
                mo7.TrueMonkey = mo6;
                mo7.FalseMonkey = mo2;

                var monkeys = new [] {mo0,
                mo1, mo2, mo3, mo4, mo5, mo6, mo7};        

                return monkeys;
        }

        private class Monkey
        {
                public List<long> Items = new List<long>();

                public Func<long, long> Operation = null;
                public Func<long, bool> Test = null;
                public Monkey TrueMonkey = null;
                public Monkey FalseMonkey = null;
                public long InspectionCount = 0;
        }
}