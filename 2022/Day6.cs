// Tuning Trouble
namespace AOC22;
public class Day6 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var signal = lines.Single();

                var last4 = new Queue<char>();

                var counter = 1;
                foreach(var c in signal)
                {
                        if(last4.Count() == 4)
                        {
                                last4.Dequeue();
                                last4.Enqueue(c);
                        }
                        else
                        {
                                last4.Enqueue(c);
                        }
                        if(last4.Distinct().Count() == 4)
                        {
                               break; 
                        }
                        counter++;
                }

                var msgCounter = 1;
                foreach(var c in signal)
                {
                        if(last4.Count() == 14)
                        {
                                last4.Dequeue();
                                last4.Enqueue(c);
                        }
                        else
                        {
                                last4.Enqueue(c);
                        }
                        if(last4.Distinct().Count() == 14)
                        {
                               break; 
                        }
                        msgCounter++;
                }

                Console.WriteLine((counter.ToString(), msgCounter.ToString()));
        }
}