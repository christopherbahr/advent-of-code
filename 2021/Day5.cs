namespace AOC21;
public class Day5 : IDay
{
        private Dictionary<(int, int), int> scoreSet = new Dictionary<(int, int), int>();
        private Dictionary<(int, int), int> scoreSet2 = new Dictionary<(int, int), int>();

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var endpoints = new List<((int x, int y) p1,(int x, int y) p2)>();
                foreach(var line in lines)
                {
                        var data = line.Split(' ');
                        var p1 = data[0].Split(',').Select(int.Parse).ToList();
                        var p2 = data[2].Split(',').Select(int.Parse).ToList();
                        endpoints.Add(((p1[0], p1[1]), (p2[0], p2[1])));
                }

                foreach(var endpoint in endpoints)
                {
                        // only do horizontal & vertical lines for now
                        var isStraight = endpoint.p1.x == endpoint.p2.x || endpoint.p1.y == endpoint.p2.y;
                        var xdiff = endpoint.p2.x - endpoint.p1.x;
                        var ydiff = endpoint.p2.y - endpoint.p1.y;
                        var (dx, dy) = (Math.Sign(xdiff), Math.Sign(ydiff));
                        var (cx, cy) = endpoint.p1;
                        while((cx, cy) != endpoint.p2)
                        {
                                AddToScoreSet((cx, cy), isStraight);
                                cx += dx;
                                cy += dy;
                        }
                        AddToScoreSet(endpoint.p2, isStraight);
                }

                var overlap1 = scoreSet.Count(x => x.Value >= 2);
                var overlap2 = scoreSet2.Count(x => x.Value >= 2);
                Console.WriteLine(overlap1);
                Console.WriteLine(overlap2);
        }

        private void AddToScoreSet((int, int) p, bool isStraight)
        {
                if(isStraight)
                {
                        if (scoreSet.ContainsKey(p))
                        {
                                scoreSet[p] += 1;
                        }
                        else
                        {
                                scoreSet[p] = 1;
                        }
                }

                if (scoreSet2.ContainsKey(p))
                {
                        scoreSet2[p] += 1;
                }
                else
                {
                        scoreSet2[p] = 1;
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