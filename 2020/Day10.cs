namespace AOC20;
public class Day10 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile).Select(int.Parse).ToArray();
                var hs = lines.ToDictionary(x => x, x => 0L);
                var last = 0;
                var oneDiff = 0;
                var threeDiff = 0;
                foreach(var entry in lines.OrderBy(x => x))
                {
                        if(entry - last == 1)
                        {
                                oneDiff++;
                        }
                        else if(entry - last == 3)
                        {
                                threeDiff++;
                        }

                        last = entry;
                }
                // add one for the final adapter
                threeDiff++;
                Console.WriteLine(oneDiff * threeDiff);

                var adapterRating = lines.Max() + 3;
                hs[0] = 1;
                hs[adapterRating] = 0;

                var q = new Queue<int>(new [] { 0 });
                var candidateRange = Enumerable.Range(1, 3);
                while(q.Any())
                {
                        var candidate = q.Dequeue();
                        foreach(var i in candidateRange)
                        {
                                var id = candidate + i;
                                var myVal = hs[candidate];
                                if(hs.ContainsKey(id))
                                {
                                        hs.AddToVal(id, myVal);
                                        if(!q.Contains(id))
                                        {
                                                q.Enqueue(id);
                                        }
                                }
                        }
                }
                Console.WriteLine(hs[adapterRating]);
        }
}