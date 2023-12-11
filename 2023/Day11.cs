namespace AOC23;
public class Day11 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var galaxies = lines.CharGrid().Where( x => x.ch == '#').Select(x => x.p).ToList();

                // Interesting note: It is extremely important to call `.ToArray()` on the end of this
                // enumerables in c# are lazy and they only expand as needed. This enumerable will be 
                // enumerated a ton of times in the tight loop below. This whole day takes about 44ms with ToArray andAbout 35s without it
                var expandR = Enumerable.Range(0, lines.Length).Where(x => !galaxies.Any(y => y.r == x)).ToArray();
                var expandC = Enumerable.Range(0, lines.First().Length).Where(x => !galaxies.Any(y => y.c == x)).ToArray();

                var p1 = 0L;
                var p2 = 0L;
                foreach(var ((r1, c1), (r2, c2)) in galaxies.TriangleIter())
                {
                        var br = Math.Max(r1, r2);
                        var sr = Math.Min(r1, r2);
                        var bc = Math.Max(c1, c2);
                        var sc = Math.Min(c1, c2);


                        // If these lists were long it would be faster to sort them
                        // and binary search for the edges and count indices
                        // They're 13 and 5 items long though so this is plenty fast
                        var ers = expandR.Count(x => x < br && x > sr);
                        var ecs = expandC.Count(x => x < bc && x > sc);

                        var md = (r1, c1).ManhattanDistance((r2, c2));

                        var d1 = md + ers + ecs;
                        // -1 because the manhattan distance already counts the expanded columns und rows once.
                        var d2 = md + (ers * (1_000_000 - 1)) + (ecs * (1_000_000 - 1));

                        p1 += d1;
                        p2 += d2;

                }
                Console.WriteLine((p1, p2));
        }
}