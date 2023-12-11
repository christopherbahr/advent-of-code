using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace AOC20;
public class Day7 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        private Dictionary<string, Bag> bagDict = new Dictionary<string, Bag>();
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                foreach(var line in lines)
                {
                        var containSplit = line.Split("contain");
                        var commaSplit = containSplit[1].Split(',');
                        var data = containSplit[0].Split(' ');
                        var bagName = data[0] + data[1];
                        var b = bagDict.GetOrAdd(bagName, () => new Bag());
                        if(commaSplit[0].Contains("no"))
                        {
                                continue;
                        }
                        foreach (var entry in commaSplit)
                        {
                                var d = entry.Trim(' ').Split(' ');
                                var count = int.Parse(d[0]);
                                var name = d[1] + d[2];
                                var cb = bagDict.GetOrAdd(name, () => new Bag());
                                b.ContainedBags.Add((cb, count));
                                cb.ContainingBags.Add(b);
                        }
                }

                var gb = bagDict["shinygold"];
                var q = new Queue<Bag>(gb.ContainingBags);
                var visited = new HashSet<Bag>();
                while(q.Any())
                {
                        var b = q.Dequeue();
                        if(visited.Contains(b))
                        {
                                continue;
                        }
                        foreach(var cb in b.ContainingBags)
                        {
                                q.Enqueue(cb);
                        }
                        visited.Add(b);
                }

                Console.WriteLine(visited.Count);
                var containedCount = CountContainedBags(gb);
                Console.WriteLine(containedCount);
        }

        private int CountContainedBags(Bag bag)
        {
                var count = 0;
                foreach(var (b, c) in bag.ContainedBags)
                {
                        count += c + c * CountContainedBags(b);
                }
                return count;
        }

        private class Bag
        {
                public IList<(Bag, int)> ContainedBags {get; set;} = new List<(Bag, int)>();
                public IList<Bag> ContainingBags {get; set;} = new List<Bag>();
        }


}