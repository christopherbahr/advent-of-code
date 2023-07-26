namespace AOC22;
public class Day10 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var cycle = 0;

                var cmds = new Queue<string>();

                foreach (var line in lines)
                {
                        cmds.Enqueue(line);
                }

                var curCmd = "";
                var x = 1;
                var score = 0;
                while (true)
                {
                        if (!cmds.Any())
                        {
                                break;
                        }
                        curCmd = cmds.Dequeue();
                        if (curCmd == "noop")
                        {
                                if (cycle != 0 && cycle % 40 == 0)
                                {
                                        Console.WriteLine();
                                }
                                if (Math.Abs(cycle % 40 - x) <= 1)
                                {
                                        Console.Write("#");
                                }
                                else
                                {
                                        Console.Write('.');
                                }
                                cycle++;
                                if (cycle == 20 || ((cycle - 20) % 40) == 0)
                                {
                                        score += cycle * x;
                                }
                                continue;
                        }
                        else
                        {
                                if (cycle != 0 && cycle % 40 == 0)
                                {
                                        Console.WriteLine();
                                }
                                if (Math.Abs(cycle % 40 - x) <= 1)
                                {
                                        Console.Write("#");
                                }
                                else
                                {
                                        Console.Write('.');
                                }
                                cycle++;
                                if (cycle == 20 || ((cycle - 20) % 40) == 0)
                                {
                                        score += cycle * x;
                                }


                                if (cycle != 0 && cycle % 40 == 0)
                                {
                                        Console.WriteLine();
                                }
                                if (Math.Abs(cycle % 40 - x) <= 1)
                                {
                                        Console.Write("#");
                                }
                                else
                                {
                                        Console.Write('.');
                                }
                                cycle++;

                                if (cycle == 20 || ((cycle - 20) % 40) == 0)
                                {
                                        score += cycle * x;
                                }
                                var inc = int.Parse(curCmd.Split(' ')[1]);
                                x += inc;
                                //Console.WriteLine($"x = {x}");

                        }
                }

                Console.WriteLine();

                // String OCRd by my own eyes and harcoded here for completeness
                Console.WriteLine( (score.ToString(), "RKAZAJBR"));
        }
}