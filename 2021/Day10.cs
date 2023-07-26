namespace AOC21;
public class Day10 : IDay
{

        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var openers = new [] { '<', '(', '[', '{'};
                var matches = new Dictionary<char, char>()
                {
                        ['{'] = '}',
                        ['('] = ')',
                        ['<'] = '>',
                        ['['] = ']',
                };

                var scoreMapP1 = new Dictionary<char, int>()
                {
                        [')'] = 3,
                        [']'] = 57,
                        ['}'] = 1197,
                        ['>'] = 25137
                };

                var scoreMapP2 = new Dictionary<char, int>()
                {
                        ['('] = 1,
                        ['['] = 2,
                        ['{'] = 3,
                        ['<'] = 4
                };

                var p1Score = 0;
                var p2Scores = new List<long>();
                foreach(var line in lines)
                {
                        long p2Score = 0;
                        var s = new Stack<char>();
                        var corrupt = false;
                        foreach(var c in line)
                        {
                                // an opener is always valid
                                if(openers.Contains(c))
                                {
                                        s.Push(c);
                                }
                                else
                                { 
                                        // we have a closer but no opener
                                        if(!s.Any())
                                        {
                                                break;
                                        }
                                        var last = s.Pop();
                                        if(matches[last] != c)
                                        {
                                                //found corruption
                                                corrupt = true;
                                                p1Score += scoreMapP1[c];
                                                break;
                                        }
                                }
                        }
                        // Not corrupt, just incomplete
                        if(!corrupt)
                        {
                                while(s.Any())
                                {
                                        p2Score *= 5;
                                        p2Score += scoreMapP2[s.Pop()];
                                }
                                p2Scores.Add(p2Score);
                        }
                }

                var orderedScores = p2Scores.OrderBy(x => x).ToList();
                var p2 = orderedScores[(orderedScores.Count() / 2)];

                Console.WriteLine(p1Score);
                Console.WriteLine(p2);
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