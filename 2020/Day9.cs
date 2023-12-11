namespace AOC20;
public class Day9 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public int preambleSize = 25;
        public void Execute()
        {
                var longs = File.ReadAllLines(inputFile).Select(long.Parse).ToArray();
                var workingSet = new List<long>(longs.Take(preambleSize));
                var lookupSet = new HashSet<long>(longs.Take(preambleSize));

                long p1Val = 0;
                foreach(var val in longs.Skip(preambleSize))
                {
                        if(workingSet.Any(x => lookupSet.Contains(val - x)))
                        {
                                lookupSet.Remove(workingSet[0]);
                                lookupSet.Add(val);
                                workingSet.Add(val);
                                workingSet.RemoveAt(0);
                                continue;
                        }
                        p1Val = val;
                        break;
                }
                Console.WriteLine(p1Val);

                // On the one hand this seems like a really naive implementation
                // but on the other if the relevant contiguous sets of numbers are
                // relatively short it won't be that expensive to rebuild them.
                //
                // Still. A much smarter implementation would track the sum and when it 
                // overshot it would remove the first item from the list and decrease the sum
                // accordingly. If it was too low it would go on adding more to the end, if it 
                // was too high it would chop from the end unlit it went back under and then start again.
                //
                // That sounds kinda state-machiney to get right. For now I'll ignore it.
                foreach(var (i, val) in longs.Enumerate())
                {
                        var acc = val;
                        var j = i;
                        while(acc < p1Val)
                        {
                                j++;
                                acc += longs[j];
                        }
                        if(acc == p1Val)
                        {
                                var min = longs[i..j].Min();
                                var max = longs[i..j].Max();
                                Console.WriteLine(min + max);
                                break;
                        }
                }
        }

}