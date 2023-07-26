namespace AOC21;
public class Day3 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var splitLines = lines.Select(x => x.Split(' '));

                var p1 = 0;
                var p2 = 0;
                var len = lines[0].Length;

                var gamma = 0;
                var setBitCount = new int[len];
                var intValues = new int[lines.Length];
                foreach(var (i, line) in Enumerate(lines))
                {
                        for(var j = 0; j < len; j++)
                        {
                                if(line[j] == '1')
                                {
                                        setBitCount[j]++;
                                }
                        }
                        intValues[i] = Convert.ToInt32(line, 2);
                }

                var epsilon = 0;
                var oxygen = 0;
                var scrub = 0;
                foreach(var (i, sbc) in Enumerate(setBitCount))
                {
                        if(sbc > lines.Length / 2)
                        {
                                gamma += 1 << (len - i - 1);
                        }
                        else
                        {
                                epsilon += 1 << (len - i - 1);
                        }
                }

                var o2List = intValues.ToList();
                var scrubList = intValues.ToList();
                for(var i = 0; i < len; i++)
                {
                        var mask = 1 << (len - i - 1);
                        var bc = 0;
                        foreach(var entry in o2List)
                        {
                                if((entry & mask) != 0)
                                {
                                        bc++;
                                }
                        }
                        var moreOnes = bc >= o2List.Count / 2.0;
                        if(moreOnes)
                        {
                                o2List = o2List.Where(x => (x & mask) != 0).ToList();
                        }
                        else
                        {
                                o2List = o2List.Where(x => (x & mask) == 0).ToList();
                        }
                        if(o2List.Count() == 1)
                        {
                                oxygen = o2List.Single();
                                break;
                        }
                }

                Console.WriteLine();

                for(var i = 0; i < len; i++)
                {
                        var mask = 1 << (len - i - 1);
                        var bc = 0;
                        foreach(var entry in scrubList)
                        {
                                if((entry & mask) != 0)
                                {
                                        bc++;
                                }
                        }
                        var moreOnes = bc >= scrubList.Count / 2.0;
                        if(moreOnes)
                        {
                                scrubList = scrubList.Where(x => (x & mask) == 0).ToList();
                        }
                        else
                        {
                                scrubList = scrubList.Where(x => (x & mask)!= 0).ToList();
                        }
                        if(scrubList.Count() == 1)
                        {
                                scrub = scrubList.Single();
                                break;
                        }
                }

                Console.WriteLine();


                Console.WriteLine(Convert.ToString(oxygen, 2));
                Console.WriteLine(Convert.ToString(scrub, 2));

                Console.WriteLine(oxygen);
                Console.WriteLine(scrub);

                p1 = gamma * epsilon;

                p2 = oxygen * scrub;

                Console.WriteLine(p1);
                Console.WriteLine(p2);

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