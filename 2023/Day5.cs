namespace AOC23;
public class Day5 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public List<IDictionary<(long sourceStart, long rangeLength),long>> dictList = new();
        public void Execute()
        {
                foreach(var _ in Enumerable.Range(0,7))
                {
                        dictList.Add(new Dictionary<(long, long),long>());
                }

                var lines = File.ReadAllLines(inputFile);
                var seeds = lines.First().GetLongs();
                var dictIdx = 0;
                var maps = lines.Split(string.Empty).Select(x => x.Skip(1)).Skip(1);
                foreach(var map in maps)
                {
                        foreach(var line in map)
                        {
                                var data = line.GetLongs();
                                dictList[dictIdx].Add((data[1], data[2]), data[0]);
                        }
                        dictIdx++;
                }

                var p1 = long.MaxValue;
                foreach(var seed in seeds)
                {
                        var cur = seed;
                        foreach(var dict in dictList)
                        {
                                // If we don't find a mapping cur doesn't change so we're fine
                                foreach(var entry in dict)
                                {
                                        if(cur >= entry.Key.sourceStart && cur < entry.Key.sourceStart + entry.Key.rangeLength)
                                        {
                                                cur = entry.Value + cur - entry.Key.sourceStart;
                                                break;
                                        }
                                }
                        }
                        p1 = Math.Min(p1, cur);
                }

                var seedRanges = new List<List<(long, long)>>();
                foreach(var i in Enumerable.Range(0, 8))
                {
                        seedRanges.Add(new List<(long, long)>());
                }
                for (var i = 0; i < seeds.Count; i+=2)
                {
                        seedRanges[0].Add((seeds[i], seeds[i+1]));
                }

                // We're probably going to want a range collapser too but for now we'll skip it
                // Result: Actually even with the many possibly adjacent ranges it doesn't matter

                foreach(var (sri, singleSeedRange) in seedRanges.Enumerate().SkipLast(1))
                {
                        foreach(var sr in singleSeedRange)
                        {
                                var dict = dictList[sri];
                                var cur = sr.Item1;
                                while(cur <= sr.Item1 + sr.Item2)
                                {
                                        long len = 0L;
                                        long start = 0L;
                                        var found = false;
                                        foreach (var entry in dict)
                                        {
                                                if (cur >= entry.Key.sourceStart && cur < entry.Key.sourceStart + entry.Key.rangeLength)
                                                {
                                                        found = true;
                                                        var offset = cur - entry.Key.sourceStart;
                                                        start = entry.Value + offset;
                                                        var targetSegmentLen = entry.Key.rangeLength - offset;
                                                        var maxSegmentLen = sr.Item1 + sr.Item2 - cur; //this would be the whole mapped segment
                                                        len = Math.Min(targetSegmentLen, maxSegmentLen);

                                                        cur += targetSegmentLen;
                                                        seedRanges[sri + 1].Add((start, len));
                                                        break;
                                                }
                                        }
                                        if(!found)
                                        {
                                                start = cur;
                                                var candidates = dict.OrderBy(x => x.Key.sourceStart).Where(x => x.Key.sourceStart > cur);
                                                if (candidates.Any())
                                                {
                                                        var end = candidates.FirstOrDefault().Key.sourceStart;
                                                        len = end - cur - 1;
                                                }
                                                else
                                                {
                                                        len = sr.Item1 + sr.Item2 - cur;
                                                }
                                                seedRanges[sri + 1].Add((start, len));
                                                cur += len + 1;
                                        }
                                }
                        }
                }
                var p2 = seedRanges[7].Select(x=> x.Item1).Min();
                Console.WriteLine((p1, p2));

        }
}
