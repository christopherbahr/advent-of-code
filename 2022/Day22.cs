// Camp Cleanup
namespace AOC22;
public class Day22 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        private static int boxWidth;
        private static int modValue;
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var height = lines.Count() - 2;
                var width = lines.First().Count(); // take advantage of full width input

                boxWidth = lines.Count() > 100 ? 50 : 4;

                modValue = boxWidth + 1;

                var paddedHeight = 4 * modValue + 1;
                var paddedWidth = 4 * modValue + 1;

                var map = new char[paddedHeight, paddedWidth];

                var lastLine = false;
                for(var i = 0; i < paddedHeight; i++)
                {
                        for(var j = 0; j < paddedWidth; j++)
                        {
                                if(i % modValue == 0 || j % modValue == 0)
                                {
                                        map[i, j] = '*';
                                        continue;
                                }
                                var io = 1 + (i / modValue);
                                var jo = 1 + (j / modValue);
                                var line = !lastLine ? lines[i-io] : null;
                                if(!lastLine && line.Count() == 0)
                                {
                                        lastLine = true;
                                }
                                if(lastLine || j-jo >= line.Count() || line[j-jo] == ' ')
                                {
                                        map[i, j] = '*';
                                }
                                else
                                {
                                        map[i,j] = line[j - jo];
                                }
                        }
                }

                // now parse directions
                var dirLine = lines.Last();
                var curPos = 0;
                var instructions = new List<Instruction>();
                while(true)
                {
                        var nextDir = dirLine.IndexOfAny(new [] { 'R', 'L'}, curPos);
                        if(nextDir == -1)
                        {
                                var ld = int.Parse(dirLine.Substring(curPos));
                                var li = new Instruction { Value = ld };
                                instructions.Add(li);
                                break;
                        }
                        var dist = int.Parse(dirLine.Substring(curPos, nextDir - curPos));
                        var walkInst = new Instruction { Value = dist};
                        instructions.Add(walkInst);

                        var dir = dirLine[nextDir];
                        var turnInst = new  Instruction {Direction = dir};
                        instructions.Add(turnInst);

                        curPos = nextDir + 1;
                }

                for(var i = 0; i < paddedWidth; i++)
                {
                        var l = "";
                        for(var j = 0; j < paddedHeight; j++)
                        {
                                l += map[i,j];
                        }
                        Console.WriteLine(l);
                }

                foreach(var inst in instructions)
                {
                        //Console.WriteLine(inst.Value.HasValue ? inst.Value.ToString() : inst.Direction.ToString());
                }


                var isP1 = true;
                var p1 = 0;

                var (cr, cc) = (1, 1);
                var (dr, dc) = (0, 1);

                while(true)
                {
                        (cr, cc) = (1, 1);
                        (dr, dc) = (0, 1);
                        for (var i = 0; i < width + 2; i++)
                        {
                                if (map[1, i] == '.')
                                {
                                        cc = i;
                                        break;
                                }
                        }
                        Console.WriteLine($"{cr} {cc}");
                        foreach (var inst in instructions)
                        {
                                var instStr = inst.Value.HasValue ? inst.Value.ToString() : inst.Direction.ToString();
                                Console.WriteLine($"Executing {instStr}");
                                if (inst.Value.HasValue)
                                {
                                        for (var i = 0; i < inst.Value; i++)
                                        {
                                                var tr = cr + dr;
                                                var tc = cc + dc;

                                                if (tr >= map.GetLength(0))
                                                {
                                                        Console.WriteLine($"Trying to execute {instStr} landed at illegal row {tr} {tc}");
                                                }
                                                if (tc >= map.GetLength(1))
                                                {
                                                        Console.WriteLine($"Trying to execute {instStr} landed at illegal col {tr} {tc}");
                                                }

                                                if (map[tr, tc] == '.')
                                                {
                                                        cr = tr;
                                                        cc = tc;
                                                }
                                                else if (map[tr, tc] == '*')
                                                {
                                                        (cr, cc, dr, dc) = GetWrap(cr, cc, dr, dc, map, isP1);
                                                }
                                                // we've hit a wall, just stop
                                                else
                                                {
                                                        break;
                                                }
                                                Console.WriteLine($"{cr} {cc} {GetDir(dr, dc)}");
                                        }
                                }
                                else
                                {
                                        switch ((dr, dc, inst.Direction))
                                        {
                                                case (0, 1, 'R'):
                                                case (0, -1, 'L'):
                                                        // point down
                                                        (dr, dc) = (1, 0);
                                                        break;
                                                case (0, 1, 'L'):
                                                case (0, -1, 'R'):
                                                        // point up
                                                        (dr, dc) = (-1, 0);
                                                        break;
                                                case (1, 0, 'R'):
                                                case (-1, 0, 'L'):
                                                        // point down
                                                        (dr, dc) = (0, -1);
                                                        break;
                                                case (1, 0, 'L'):
                                                case (-1, 0, 'R'):
                                                        // point up
                                                        (dr, dc) = (0, 1);
                                                        break;
                                        }
                                        Console.WriteLine($"{cr} {cc} {GetDir(dr, dc)}");
                                }

                        }
                        if(isP1)
                        {
                                isP1 = false;
                                var p1fr = cr - (cr / modValue);
                                var p1fc = cc - (cc / modValue);
                                var p1facing = -1;
                                switch ((dr, dc))
                                {
                                        case (0, 1):
                                                p1facing = 0;
                                                break;
                                        case (0, -1):
                                                p1facing = 2;
                                                break;
                                        case (1, 0):
                                                p1facing = 1;
                                                break;
                                        case (-1, 0):
                                                p1facing = 3;
                                                break;
                                }
                                p1 = 1000 * p1fr + 4 * p1fc + p1facing;
                        }
                        else
                        {
                                break;
                        }
                }

                var finalRow = cr - (cr / modValue);
                var finalCol = cc - (cc / modValue);
                var facing = -1;
                switch ((dr, dc))
                {
                        case (0, 1):
                                facing = 0;
                                break;
                        case (0, -1):
                                facing = 2;
                                break;
                        case (1, 0):
                                facing = 1;
                                break;
                        case (-1, 0):
                                facing = 3;
                                break;
                }

                Console.WriteLine($"{finalRow} {finalCol} {facing}");

                var pwd = 1000 * finalRow + 4 * finalCol + facing;

                Console.WriteLine( (p1.ToString(), pwd.ToString()));
        }

        private (int, int) GetDir(char dir)
        {
                switch (dir)
                {
                        case 'R':
                                return (0, 1);
                        case 'L':
                                return (0, -1);
                        case 'U':
                                return (-1, 0);
                        case 'D':
                                return (1, 0);
                }
                throw new Exception("bad dir");
        }

        private char GetDir(int dr, int dc)
        {
                switch (dr, dc)
                {
                        case (0, 1):
                                return 'R';
                        case (0, -1):
                                return 'L';
                        case (1, 0):
                                return 'D';
                        case (-1, 0):
                                return 'U';
                }
                throw new Exception("bad dir 2");
        }

        private (int r, int c, int dr, int dc) GetWrap(int r, int c, int dr, int  dc, char[,] map, bool p1)
        {
                // first figure out which box it's in

                var br = r / modValue;
                var bc = c / modValue;

                //box local row/column
                var blr = r % modValue - 1;
                var blc = c % modValue - 1;

                var dirChar = GetDir(dr, dc);

                var cm = p1 ? p1CornerMap : cornerMap;
                if(map.GetLength(0) < 100)
                {
                        cm = sampleCornerMap;
                }

                var (tbr, tbc, td) = cm[(br, bc, dirChar)];

                int tblr, tblc = 0;
                var useBw = boxWidth - 1;
                switch((dirChar, td))
                {
                        case ('L', 'L'):
                                tblr = blr;
                                tblc = useBw;
                                break;
                        case ('L', 'R'):
                                tblr = useBw - blr;
                                tblc = 0;
                                break;
                        case ('L', 'U'):
                                tblr = useBw;
                                tblc = useBw - blr;
                                break;
                        case ('L', 'D'):
                                tblr = 0;
                                tblc = blr;
                                break;

                        case ('R', 'L'):
                                tblr = useBw - blr;
                                tblc = useBw;
                                break;
                        case ('R', 'R'):
                                tblr = blr;
                                tblc = 0;
                                break;
                        case ('R', 'U'):
                                tblr = useBw;
                                tblc = blr;
                                break;
                        case ('R', 'D'):
                                tblr = 0;
                                tblc = useBw - blr;
                                break;

                        case ('U', 'L'):
                                tblr = useBw - blc;
                                tblc = useBw;
                                break;
                        case ('U', 'R'):
                                tblr = blc;
                                tblc = 0;
                                break;
                        case ('U', 'U'):
                                tblr = useBw;
                                tblc = blc;
                                break;
                        case ('U', 'D'):
                                tblr = 0;
                                tblc = useBw - blc;
                                break;

                        case ('D', 'L'):
                                tblr = blc;
                                tblc = useBw;
                                break;
                        case ('D', 'R'):
                                tblr = useBw - blc;
                                tblc = 0;
                                break;
                        case ('D', 'U'):
                                tblr = useBw;
                                tblc = useBw - blc;
                                break;
                        case ('D', 'D'):
                                tblr = 0;
                                tblc = blc;
                                break;
                        default:
                                throw new Exception("bad dircombo");
                }


                var passable = true;

                var tr = (modValue * tbr) + 1 + tblr;
                var tc = (modValue * tbc) + 1 + tblc;
                passable = map[tr, tc] == '.';
                if(passable)
                {
                        var (tdr, tdc) = GetDir(td);
                        return (tr, tc, tdr, tdc);
                }
                else
                {
                        return (r, c, dr, dc);
                }
        }

        private class Instruction
        {
                public int? Value;
                public char? Direction;

        }

        
        private Dictionary<(int, int, char), (int br, int bc, char dir)> p1CornerMap = new Dictionary<(int, int, char), (int br, int bc, char dir)>
        {
                [(0, 1, 'R')] = (0, 2, 'R'),
                [(0, 1, 'L')] = (0, 2, 'L'),
                [(0, 1, 'U')] = (2, 1, 'U'),
                [(0, 1, 'D')] = (1, 1, 'D'),

                [(0, 2, 'R')] = (0, 1, 'R'),
                [(0, 2, 'L')] = (0, 1, 'L'),
                [(0, 2, 'U')] = (0, 2, 'U'),
                [(0, 2, 'D')] = (0, 2, 'D'),

                [(1, 1, 'R')] = (1, 1, 'R'),
                [(1, 1, 'L')] = (1, 1, 'L'),
                [(1, 1, 'U')] = (0, 1, 'U'),
                [(1, 1, 'D')] = (2, 1, 'D'),

                [(2, 0, 'R')] = (2, 1, 'R'),
                [(2, 0, 'L')] = (2, 1, 'L'),
                [(2, 0, 'U')] = (3, 0, 'U'),
                [(2, 0, 'D')] = (3, 0, 'D'),

                [(2, 1, 'R')] = (2, 0, 'R'),
                [(2, 1, 'L')] = (2, 0, 'L'),
                [(2, 1, 'U')] = (1, 1, 'U'),
                [(2, 1, 'D')] = (0, 1, 'D'),

                [(3, 0, 'R')] = (3, 0, 'R'),
                [(3, 0, 'L')] = (3, 0, 'L'),
                [(3, 0, 'U')] = (2, 0, 'U'),
                [(3, 0, 'D')] = (2, 0, 'D'),
        };

        private Dictionary<(int, int, char), (int br, int bc, char dir)> cornerMap = new Dictionary<(int, int, char), (int br, int bc, char dir)>
        {
                [(0, 1, 'R')] = (0, 2, 'R'),
                [(0, 1, 'L')] = (2, 0, 'R'),
                [(0, 1, 'U')] = (3, 0, 'R'),
                [(0, 1, 'D')] = (1, 1, 'D'),

                [(0, 2, 'R')] = (2, 1, 'L'),
                [(0, 2, 'L')] = (0, 1, 'L'),
                [(0, 2, 'U')] = (3, 0, 'U'),
                [(0, 2, 'D')] = (1, 1, 'L'),

                [(1, 1, 'R')] = (0, 2, 'U'),
                [(1, 1, 'L')] = (2, 0, 'D'),
                [(1, 1, 'U')] = (0, 1, 'U'),
                [(1, 1, 'D')] = (2, 1, 'D'),

                [(2, 0, 'R')] = (2, 1, 'R'),
                [(2, 0, 'L')] = (0, 1, 'R'),
                [(2, 0, 'U')] = (1, 1, 'R'),
                [(2, 0, 'D')] = (3, 0, 'D'),

                [(2, 1, 'R')] = (0, 2, 'L'),
                [(2, 1, 'L')] = (2, 0, 'L'),
                [(2, 1, 'U')] = (1, 1, 'U'),
                [(2, 1, 'D')] = (3, 0, 'L'),

                [(3, 0, 'R')] = (2, 1, 'U'),
                [(3, 0, 'L')] = (0, 1, 'D'),
                [(3, 0, 'U')] = (2, 0, 'U'),
                [(3, 0, 'D')] = (0, 2, 'D'),
        };

        private Dictionary<(int, int, char), (int br, int bc, char dir)> sampleCornerMap = new Dictionary<(int, int, char), (int br, int bc, char dir)>
        {
                [(0, 2, 'R')] = (2, 3, 'R'),
                [(0, 2, 'L')] = (1, 1, 'D'),
                [(0, 2, 'U')] = (1, 0, 'R'),
                [(0, 2, 'D')] = (1, 2, 'D'),

                [(1, 2, 'R')] = (2, 3, 'D'),
                [(1, 2, 'L')] = (1, 1, 'L'),
                [(1, 2, 'U')] = (0, 2, 'U'),
                [(1, 2, 'D')] = (2, 2, 'D'),

                [(2, 2, 'R')] = (2, 3, 'R'),
                [(2, 2, 'L')] = (1, 1, 'U'),
                [(2, 2, 'U')] = (1, 2, 'U'),
                [(2, 2, 'D')] = (1, 0, 'U'),

                [(1, 0, 'R')] = (1, 1, 'R'),
                [(1, 0, 'L')] = (2, 3, 'U'),
                [(1, 0, 'U')] = (0, 2, 'D'),
                [(1, 0, 'D')] = (2, 2, 'U'),

                [(1, 1, 'R')] = (1, 2, 'R'),
                [(1, 1, 'L')] = (1, 0, 'L'),
                [(1, 1, 'U')] = (0, 2, 'R'),
                [(1, 1, 'D')] = (2, 2, 'R'),

                [(2, 3, 'R')] = (0, 2, 'L'),
                [(2, 3, 'L')] = (2, 2, 'L'),
                [(2, 3, 'U')] = (1, 2, 'L'),
                [(2, 3, 'D')] = (1, 0, 'R'),
        };

        // Rulas

        // R <-> L => Invert 
        // There are no U <-> D but that would invert too
        // U <-> R & L <-> D => Col <-> Row but no invert

        // (0, 1, R) => (0, 2, R)
        // (0, 1, L) => (2, 0, R) invert
        // (0, 1, U) => (3, 0, R)
        // (0, 1, D) => (1, 1, D)

        // (0, 2, R) => (2, 1, L)
        // (0, 2, L) => (0, 1, L)
        // (0, 2, U) => (3, 0, U)
        // (0, 2, D) => (1, 1, L)

        // (1, 1, R) => (0, 2, U)
        // (1, 1, L) => (2, 0, D)
        // (1, 1, U) => (0, 1, U)
        // (1, 1, D) => (2, 1, D)

        // (2, 0, R) => (2, 1, R)
        // (2, 0, L) => (0, 1, R)
        // (2, 0, U) => (1, 1, R)
        // (2, 0, D) => (3, 0, D)

        // (2, 1, R) => (0, 2, L)
        // (2, 1, L) => (2, 0, L)
        // (2, 1, U) => (1, 1, U)
        // (2, 1, D) => (3, 0, L)

        // (3, 0, R) => (2, 1, U)
        // (3, 0, L) => (0, 1, D)
        // (3, 0, U) => (2, 0, U)
        // (3, 0, D) => (0, 2, D)

}