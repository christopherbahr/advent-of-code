using System.Numerics;

namespace AOC21;
public class Day6 : IDay
{
        private Dictionary<(int, int), BigInteger> cache = new Dictionary<(int, int), BigInteger>();
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                // The key realization here is that we don't need to simulate a population of fish
                // Each fish's lineage is independent so we can simulate each fish in turn and memoize

                var f = lines.First().Split(',').Select(int.Parse).ToList();
                BigInteger sum = 0;
                BigInteger sum2 = 0;
                foreach(var fish in f)
                {
                        sum += SimulateFish(fish, 80);
                }

                foreach(var fish in f)
                {
                        sum2 += SimulateFish(fish, 256);
                }

                Console.WriteLine(sum);
                Console.WriteLine(sum2);

                
        }

        private BigInteger SimulateFish(int fish, int daysLeft)
        {
                if(fish >= daysLeft || daysLeft <= 0)
                {
                        return 1;
                }
                if(cache.ContainsKey((fish, daysLeft))) 
                {
                        return cache[(fish, daysLeft)];
                }
                if(fish == 0)
                {
                        BigInteger thisFish= SimulateFish(0, daysLeft - 7);
                        BigInteger newFish = SimulateFish(0, daysLeft - 9);
                        cache[(fish, daysLeft)] = thisFish + newFish;
                        return (thisFish + newFish);
                }
                else
                {
                        // jump ahead to the next time we'll see this
                        BigInteger retVal = SimulateFish(0, daysLeft - fish);
                        cache[(fish, daysLeft)] = retVal;
                        return retVal;
                }
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