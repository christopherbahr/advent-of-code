namespace AOC21;
public class Day14 : IDay
{
        private Dictionary<(char, char), char> subDict = new Dictionary<(char,char), char>();
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var template = lines.First();
                foreach(var line in lines.Skip(2))
                {
                        var data = line.Split(" -> ");
                        subDict[(data[0][0], data[0][1])] = data[1].Single();
                }
                var allCounts = new Dictionary<char, long>();
                foreach(var c in template)
                {
                        allCounts.AddToVal(c, 1);
                }
                for(var j = 0; j < template.Length - 1; j++)
                {
                        var d = GetCount(template[j], template[j+1], 10);
                        foreach(var (k,v) in d)
                        {
                                allCounts.AddToVal(k, v);
                        }
                }

                var max = allCounts.Max(x => x.Value);
                var min = allCounts.Min(x => x.Value);

                Console.WriteLine(max - min);

                allCounts.Clear();

                foreach(var c in template)
                {
                        allCounts.AddToVal(c, 1);
                }

                for(var j = 0; j < template.Length - 1; j++)
                {
                        var d = GetCount(template[j], template[j+1], 40);
                        foreach(var (k,v) in d)
                        {
                                allCounts.AddToVal(k, v);
                        }
                }

                max = allCounts.Max(x => x.Value);
                min = allCounts.Min(x => x.Value);

                Console.WriteLine(max - min);
        }

        private Dictionary<(char, char, int), Dictionary<char, long>> cache = new Dictionary<(char, char, int), Dictionary<char, long>>();

        private Dictionary<char, long> GetCount(char s1, char s2, int stepsLeft)
        {
                var key = (s1, s2, stepsLeft);
                if(cache.ContainsKey(key))
                {
                        return cache[key];
                }
                var counts = new Dictionary<char, long>();
                if(stepsLeft == 0)
                {
                        return counts;
                }

                var toAdd = subDict[(s1, s2)];
                counts[toAdd] = 1;

                var s1Add = GetCount(s1, toAdd, stepsLeft - 1);
                var s2Add = GetCount(toAdd, s2, stepsLeft - 1);
                foreach(var (k,v) in s1Add.Concat(s2Add))
                {
                        counts.AddToVal(k, v);
                }

                cache[key] = counts;
                return counts;
        }
}