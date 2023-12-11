namespace AOC23;
public class Day4 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var sum = 0L;
                var dict = new Dictionary<int, long>();
                foreach(var (i, line) in lines.Enumerate())
                {
                        dict.AddToVal(i, 1);
                        var d = line.Split(':')[1].Split('|');
                        var winners = d[0].GetInts();
                        var numbers = d[1].GetInts();
                        var matchCount = numbers.Count(x => winners.Contains(x));
                        var cs = matchCount == 0 ? 0 : Math.Pow(2, matchCount - 1);
                        sum += Convert.ToInt64(cs);
                        foreach(var n in Enumerable.Range(i + 1, matchCount))
                        {
                                dict.AddToVal(n, dict[i]);
                        }
                }
                var cardCount = dict.Aggregate(0L, (acc, val) => acc + val.Value);
                Console.WriteLine((sum, cardCount));

        }
}
