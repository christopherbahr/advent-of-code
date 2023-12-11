namespace AOC23;
public class Day9 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadLines(inputFile);
                var p1 = 0L;
                var p2 = 0L;
                foreach (var line in lines)
                {
                        //We'll want to deal with the difference lists in reverse order
                        // so put them on a stack to get LIFO behavior
                        var ll = new Stack<List<int>>();
                        var l = line.GetInts();
                        ll.Push(l);
                        // no need to actually calculate the row where everything is 0,
                        // the first row where they're all the same is the last relevant one
                        while(!l.All(x => x == l.First()))
                        {
                                var nl = new List<int>();
                                foreach(var i in Enumerable.Range(0, l.Count - 1))
                                {
                                        var d = l[i+1] - l[i];
                                        nl.Add(d);
                                }
                                ll.Push(nl);
                                l = nl;
                        }
                        var (c11, c22) = ll.AsEnumerable().Aggregate((0, 0), (acc, x) => (x.Last() + acc.Item1, x.First() - acc.Item2));
                        p1 += c11;
                        p2 += c22;
                }
                Console.WriteLine((p1, p2));
        }
}