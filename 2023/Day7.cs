namespace AOC23;
public class Day7 : IDay
{
    public string inputFile { get; set; } = "wip.in";
    public void Execute()
    {
        var lines = File.ReadAllLines(inputFile);
        var hands = new List<Hand>();
        foreach (var line in lines)
        {
            var h = new Hand();
            hands.Add(h);
            foreach (var c in line.Split(' ')[0])
            {
                h.Cards.Add(c);
            }
            h.bid = line.Split(' ')[1].GetInts().First();
        }

        hands.Sort(new LambdaComparer<Hand>((h1, h2) =>
        {
            var t1 = h1.TypeScore();
            var t2 = h2.TypeScore();
            var typeComp = t1.CompareTo(t2);
            if (typeComp != 0)
            {
                return typeComp;
            }
            else
            {
                foreach (var i in Enumerable.Range(0, 5))
                {
                    var fc1 = h1.CardScore(i);
                    var fc2 = h2.CardScore(i);
                    var comp = fc1.CompareTo(fc2);
                    if (comp != 0)
                    {
                        return comp;
                    }

                }
            }
            return 0;
        }));

        var totalScore = 0L;
        foreach (var (i, h) in hands.Enumerate())
        {
            totalScore += (i + 1) * h.bid;
        }
        Console.WriteLine(totalScore);

        hands.Sort(new LambdaComparer<Hand>((h1, h2) =>
        {
            var t1 = h1.TypeScore2();
            var t2 = h2.TypeScore2();
            var typeComp = t1.CompareTo(t2);
            if (typeComp != 0)
            {
                return typeComp;
            }
            else
            {
                foreach (var i in Enumerable.Range(0, 5))
                {
                    var fc1 = h1.CardScore2(i);
                    var fc2 = h2.CardScore2(i);
                    var comp = fc1.CompareTo(fc2);
                    if (comp != 0)
                    {
                        return comp;
                    }
                }
            }
            return 0;
        }));

        var totalScore2 = 0L;
        foreach (var (i, h) in hands.Enumerate())
        {
            totalScore2 += (i + 1) * h.bid;
        }
        Console.WriteLine(totalScore2);
    }

    public class Hand
    {
        public List<char> Cards = new List<char>();
        public int bid;

        private Dictionary<char, int> scoreDict = new Dictionary<char, int>
        {
            ['T'] = 10,
            ['J'] = 11,
            ['Q'] = 12,
            ['K'] = 13,
            ['A'] = 14
        };

        private Dictionary<char, int> scoreDict2 = new Dictionary<char, int>
        {
            ['T'] = 10,
            ['J'] = 0,
            ['Q'] = 12,
            ['K'] = 13,
            ['A'] = 14
        };

        public int CardScore2(int toCheck)
        {
            var ctc = Cards[toCheck];
            if (ctc.IsDigit())
            {
                var (_, cs) = ctc.GetDigit();
                return cs;
            }
            else
            {
                return scoreDict2[ctc];
            }
        }

        public int CardScore(int toCheck)
        {
            var ctc = Cards[toCheck];
            if (ctc.IsDigit())
            {
                var (_, cs) = ctc.GetDigit();
                return cs;
            }
            else
            {
                return scoreDict[ctc];
            }
        }

        public int TypeScore2()
        {
            var d = new Dictionary<char, int>();

            foreach (var card in Cards)
            {
                d.AddToVal(card, 1);
            }
            var vals = d.Where(x => x.Key != 'J').Select(x => x.Value).OrderByDescending(x => x);
            var nonJval = vals.FirstOrDefault();
            var jVal = 0;
            d.TryGetValue('J', out jVal);
            var highVal = nonJval + jVal;
            return TypeScoreInternal(highVal, vals);
        }

        private int TypeScoreInternal(int highVal, IEnumerable<int> vals)
        {
            var tScore = 0;
            if (highVal == 5)
            {
                tScore = 6;
            }
            else if (highVal == 4)
            {
                tScore = 5;
            }
            else if (highVal == 3)
            {
                if (vals.Skip(1).Contains(2))
                {
                    tScore = 4;
                }
                else
                {
                    tScore = 3;
                }
            }
            else if (highVal == 2)
            {
                if (vals.Count(x => x == 2) == 2)
                {
                    tScore = 2;
                }
                else
                {
                    tScore = 1;
                }
            }

            return tScore;
        }

        public int TypeScore()
        {
            var d = new Dictionary<char, int>();

            foreach (var card in Cards)
            {
                d.AddToVal(card, 1);
            }
            var vals = d.Values.OrderByDescending(x => x);
            var highVal = vals.First();
            return TypeScoreInternal(highVal, vals);
        }

    }
}
