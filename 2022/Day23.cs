using System.Diagnostics;

// this could be even faster if we stored occupied as an array of BitArrays so we could just use masks for everything
// Maybe even better is a 2d array of BitVector32, you have to manage boundaries though which is a nightmare
namespace AOC22;
public class Day23 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        public void Execute()
        {

                var lines = File.ReadAllLines(inputFile);

                var height = lines.Count();
                var width = lines[0].Count();

                var elves = new List<Elf>();

                var rOffset = 20;
                var cOffset = 20;

                int[,] occupiedArr = new int[height + 2 * rOffset, width + 2 * cOffset];

                var dirs = new (int dr, int dc)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

                // Up, down, left, right
                var dirMasks = new byte[] { 
                        (1 << 7) + (1 << 0) + (1 << 1),
                        (1 << 5) + (1 << 4) + (1 << 3),
                        (1 << 5) + (1 << 6) + (1 << 7), 
                        (1 << 1) + (1 << 2) + (1 << 3),
                        };

                var lDirs = new (int dr, int dc)[] { (0, -1), (1, -1), (-1, -1)};
                var rDirs = new (int dr, int dc)[] { (0, 1), (1, 1), (-1, 1)};
                var uDirs = new (int dr, int dc)[] { (-1, 0), (-1, 1), (-1, -1)};

                var allDirs = new (int dr, int dc)[] {
                (-1, 0),
                (-1, 1),
                (0, 1),
                (1, 1),
                (1, 0),
                (1, -1),
                (0, -1),
                (-1, -1)
                };
                var dirPointer = 0;

                for (var i = 0; i < height; i++)
                {
                        for (var j = 0; j < width; j++)
                        {
                                var c = lines[i][j];
                                if (c == '#')
                                {
                                        occupiedArr[i + rOffset, j + rOffset] = 1;
                                        var e = new Elf { CurrentPos = (i + rOffset, j + cOffset) };
                                        elves.Add(e);
                                }
                        }
                }

                var moved = true;

                var counter = 0;
                var p1 = 0;
                var p2 = 0;

                var minR = int.MaxValue;
                var maxR = 0;
                var minC = int.MaxValue;
                var maxC = 0;
                while (moved)
                {
                        //PrettyPrint(occupiedArr);
                        counter++;
                        moved = false;
                        var desiredPoints = new int[occupiedArr.GetLength(0),occupiedArr.GetLength(1)];
                        foreach (var elf in elves)
                        {
                                var (cr, cc) = elf.CurrentPos;
                                var (dr, dc) = (0, 0);
                                var mustMove = false;
                                byte dirMask = 0;
                                for(int i = 0; i < allDirs.Length; i++)
                                {
                                        var (tdr, tdc) = allDirs[i];
                                        if (occupiedArr[cr + tdr, cc + tdc] == 1)
                                        {
                                                dirMask += (byte)(1 << i);
                                                mustMove = true;
                                        }
                                }
                                if(mustMove)
                                {
                                        for (var dp = dirPointer; dp < dirPointer + 4; dp++)
                                        {
                                                var mask = dirMasks[dp % dirMasks.Length];
                                                var (ddr, ddc) = dirs[dp % dirs.Length];

                                                var canMove = (mask & dirMask) == 0;

                                                if (canMove)
                                                {
                                                        dr = ddr;
                                                        dc = ddc;
                                                        break;
                                                }
                                        }
                                }
                                elf.DesiredPos = (cr + dr, cc + dc);
                                desiredPoints[cr + dr, cc + dc] += 1;
                        }

                        foreach (var elf in elves)
                        {
                                if(elf.DesiredPos == elf.CurrentPos)
                                {
                                        continue;
                                }

                                if(desiredPoints[elf.DesiredPos.nr, elf.DesiredPos.nc] > 1)
                                {
                                        continue;
                                }

                                moved = true;
                                minR = Math.Min(elf.DesiredPos.nr, minR);
                                minC = Math.Min(elf.DesiredPos.nc, minC);

                                maxR = Math.Max(elf.DesiredPos.nr, maxR);
                                maxC = Math.Max(elf.DesiredPos.nc, maxC);

                                occupiedArr[elf.CurrentPos.r, elf.CurrentPos.c] = 0;
                                elf.CurrentPos = elf.DesiredPos;
                                occupiedArr[elf.CurrentPos.r, elf.CurrentPos.c] = 1;
                        }

                        // resize the array as needed
                        if(minC == 1 || minR == 1 || maxC == occupiedArr.GetLength(1) - 2 || maxR == occupiedArr.GetLength(0) - 2)
                        {
                                var h = maxR - minR;
                                var w = maxC - minC;
                                var fromSize = occupiedArr.GetLength(0);
                                cOffset += 40;
                                rOffset += 40;
                                occupiedArr = new int[h + 2 * rOffset, w + 2*cOffset];
                                foreach(var elf in elves)
                                {
                                        var pp = elf.CurrentPos;
                                        elf.CurrentPos = (elf.CurrentPos.r + rOffset, elf.CurrentPos.c + cOffset);

                                        occupiedArr[elf.CurrentPos.r, elf.CurrentPos.c] = 1;
                                }
                        }

                        dirPointer = (dirPointer + 1) % dirs.Length;
                        //PrettyPrint(occupied);
                        if (counter == 9)
                        {
                                var area = (maxC - minC + 1) * (maxR - minR + 1);

                                p1 = area - elves.Count();
                        }
                }

                p2 = counter;

                //PrettyPrint(occupied);
                Console.WriteLine( (p1.ToString(), p2.ToString()));

        }

        void PrettyPrint(int[,] occupied)
        {
                var minR = occupied.GetLength(1) - 1;
                var maxR = 0;

                var minC = occupied.GetLength(0) - 1;
                var maxC = 0;

                for(var i = 0; i< occupied.GetLength(0); i++)
                {
                        for (var j = 0; j < occupied.GetLength(1); j++)
                        {
                                if(occupied[i,j] == 1)
                                {
                                        minR = Math.Min(i, minR);
                                        maxR = Math.Max(i, maxR);
                                        minC = Math.Min(j, minC);
                                        maxC = Math.Max(j, maxC);
                                }
                        }
                }

                for (var i = minR; i < maxR + 1; i++)
                {
                        var line = "";
                        for (var j = minC; j < maxC + 1; j++)
                        {
                                line += occupied[i,j] == 1 ? '#' : '.';
                        }
                        Console.WriteLine(line);
                }
                Console.WriteLine();
        }

        public class Elf
        {
                public (int r, int c) CurrentPos;
                public (int nr, int nc) DesiredPos = (-1, -1);
        }
}
