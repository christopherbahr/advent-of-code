namespace AOC21;
public class Day4 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var splitLines = lines.Select(x => x.Split(' '));
                var len = lines[0].Length;

                var p1 = 0;
                var p2 = 0;
                var calls = lines.First().Split(',').Select(int.Parse);

                var boards = new List<((int, bool)[,], bool)>();

                var board = new (int, bool)[5, 5];
                var i = 0;
                foreach (var (_, line) in Enumerate(lines.Skip(2)))
                {
                        if (line == string.Empty)
                        {
                                boards.Add((board, false));
                                board = new (int, bool)[5, 5];
                                continue;
                        }

                        foreach (var (j, num) in Enumerate(line.Split(' ').Where(x => x != string.Empty)))
                        {
                                var inum = int.Parse(num.Trim());
                                board[i, j] = (inum, false);
                        }

                        i++;
                        i = i % 5;
                }

                boards.Add((board, false));
                var wonBoards = new List<int>();

                Console.WriteLine(boards.Count());

                var lastWinningCall = 0;
                foreach (var call in calls)
                {
                        foreach (var (bn, (b, w)) in Enumerate(boards))
                        {
                                if (wonBoards.Contains(bn))
                                {
                                        continue;
                                }
                                for (var r = 0; r < 5; r++)
                                {
                                        for (var c = 0; c < 5; c++)
                                        {
                                                if (b[r, c].Item1 == call)
                                                {
                                                        b[r, c].Item2 = true;
                                                }
                                        }
                                }

                                for (var r = 0; r < 5; r++)
                                {
                                        var marked = true;
                                        for (var c = 0; c < 5; c++)
                                        {
                                                marked &= b[r, c].Item2;
                                        }
                                        if (marked)
                                        {
                                                p1 = p1 == 0 ? ScoreBoard(b, call) : p1;
                                                wonBoards.Add(bn);
                                                lastWinningCall = call;
                                        }
                                }

                                for (var c = 0; c < 5; c++)
                                {
                                        var marked = true;
                                        for (var r = 0; r < 5; r++)
                                        {
                                                marked &= b[r, c].Item2;
                                        }
                                        if (marked)
                                        {
                                                p1 = p1 == 0 ? ScoreBoard(b, call) : p1;
                                                wonBoards.Add(bn);
                                                lastWinningCall = call;
                                        }
                                }
                        }
                }
                p2 = ScoreBoard(boards[wonBoards.Last()].Item1, lastWinningCall);



                Console.WriteLine(p1);
                Console.WriteLine(p2);
        }

        private int ScoreBoard((int, bool)[,] board, int call)
        {
                var sum = 0;
                for (var i = 0; i < 5; i++)
                {
                        for (var j = 0; j < 5; j++)
                        {
                                if (!board[i, j].Item2)
                                {
                                        sum += board[i, j].Item1;
                                }
                        }
                }
                return sum * call;
        }

        private IEnumerable<(int, T)> Enumerate<T>(IEnumerable<T> input)
        {
                var i = 0;
                foreach (var thing in input)
                {
                        yield return (i, thing);
                        i++;
                }
        }

}