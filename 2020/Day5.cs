using System.Numerics;

namespace AOC20;
public class Day5 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var maxId = 0;
                var idList = new HashSet<int>();
                foreach(var line in lines)
                {
                        var low = 0;
                        var high = 127;
                        for(var i = 0; i < 7; i++)
                        {
                                var mp = low + ((high - low) / 2);
                                if(line[i] == 'F')
                                {
                                        high = mp;
                                }
                                else
                                {
                                        low = mp + 1;
                                }
                        }
                        if(high != low)
                        {
                                Console.WriteLine($"Mismatch {high} {low}");
                        }
                        
                        var colLow = 0;
                        var colHigh = 7;
                        for(var i = 0; i < 3; i++)
                        {
                                var mp = colLow + ((colHigh - colLow) / 2);
                                if(line[i+7] == 'L')
                                {
                                        colHigh = mp;
                                }
                                else
                                {
                                        colLow = mp + 1;
                                }
                        }
                        if(colHigh != colLow)
                        {
                                Console.WriteLine($"Col Mismatch {colHigh} {colLow}");
                        }

                        var id = high * 8 + colHigh;
                        idList.Add(id);
                        maxId = Math.Max(id, maxId);
                }
                Console.WriteLine(maxId);

                // It might be faster to iterate the sorted list and look for 2 ids with a gap match but this is easier
                for(var i = 0; i < maxId; i++)
                {
                        if(idList.Contains(i - 1) && idList.Contains(i+1) && !idList.Contains(i))
                        {
                                Console.WriteLine(i);
                                break;
                        }
                }

        }

}