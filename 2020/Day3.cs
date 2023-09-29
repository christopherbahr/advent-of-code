using System.Numerics;

namespace AOC20;
public class Day3 : IDay
{
        int maxRow, maxC;
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var g = lines.AsCharGrid();
                maxRow = lines.Count();
                maxC = lines.First().Count();

                var p1 = CheckSlope(1, 3, g);
                Console.WriteLine(p1);

                var slopes = new [] { (1, 1), (1, 3), (1, 5), (1, 7), (2, 1)};
                BigInteger p2 = 1L;
                foreach(var (dr, dc) in slopes)
                {
                        var slope = CheckSlope(dr, dc, g);
                        p2 *= slope;
                }
                Console.WriteLine(p2);

        }
        
        private int CheckSlope(int dr, int dc, char[,] g)
        {
                var counter = 0;
                var (r, c) = (0, 0);
                while(r < maxRow - dr)
                {
                        var cr = r + dr;
                        var cc = (c + dc) % maxC;
                        if(g[cr,cc] == '#')
                        {
                                counter++;
                        }
                        r = cr;
                        c = cc;
                }
                return counter;
        }

}