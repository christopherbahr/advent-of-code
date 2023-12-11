using System.Numerics;

namespace AOC20;
public class Day13 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var departureTime = int.Parse(lines[0]);
                var busses = lines[1].GetInts();
                var minTime = departureTime;
                var firstBus = busses.First();
                foreach (var bus in busses)
                {
                        var waitTime = bus - (departureTime % bus);
                        if (waitTime < minTime)
                        {
                                minTime = waitTime;
                                firstBus = bus;
                        }
                }
                Console.WriteLine(firstBus * minTime);


                // Get each bus and its position in the line
                // Then calculate how long before the target time the last bus should have left such that
                // the next time it leaves will align with it's position
                // The complicated modulo thing in the middle is to deal with the fact that 
                // % in C# is actually remainder not modulo so it can return negative numbers
                // (a % m + m) % m guarantees a positive number
                var busWithOffset = lines[1].Split(',').Enumerate().Where(x => x.val != "x").Select(x => (x.i, int.Parse(x.val)))
                                        .Select(x => (x.Item2, (((x.Item2 - x.i) % x.Item2) + x.Item2) % x.Item2));

                var mx = CRTCalculator.CRT(busWithOffset);
                Console.WriteLine(mx);
        }


}