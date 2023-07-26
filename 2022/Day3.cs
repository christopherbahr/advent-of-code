// Rucksack Reorg
namespace AOC22;
public class Day3 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        private Dictionary<long, int> logDict = new Dictionary<long, int>
        {
                [(long)1 << 0] = 0,
                [(long)1 << 1] = 1,
                [(long)1 << 2] = 2,
                [(long)1 << 3] = 3,
                [(long)1 << 4] = 4,
                [(long)1 << 5] = 5,
                [(long)1 << 6] = 6,
                [(long)1 << 7] = 7,
                [(long)1 << 8] = 8,
                [(long)1 << 9] = 9,
                [(long)1 << 10] = 10,
                [(long)1 << 11] = 11,
                [(long)1 << 12] = 12,
                [(long)1 << 13] = 13,
                [(long)1 << 14] = 14,
                [(long)1 << 15] = 15,
                [(long)1 << 16] = 16,
                [(long)1 << 17] = 17,
                [(long)1 << 18] = 18,
                [(long)1 << 19] = 19,
                [(long)1 << 20] = 20,
                [(long)1 << 21] = 21,
                [(long)1 << 22] = 22,
                [(long)1 << 23] = 23,
                [(long)1 << 24] = 24,
                [(long)1 << 25] = 25,
                [(long)1 << 26] = 26,
                [(long)1 << 27] = 27,
                [(long)1 << 28] = 28,
                [(long)1 << 29] = 29,
                [(long)1 << 30] = 30,
                [(long)1 << 31] = 31,
                [(long)1 << 32] = 32,
                [(long)1 << 33] = 33,
                [(long)1 << 34] = 34,
                [(long)1 << 35] = 35,
                [(long)1 << 36] = 36,
                [(long)1 << 37] = 37,
                [(long)1 << 38] = 38,
                [(long)1 << 39] = 39,
                [(long)1 << 40] = 40,
                [(long)1 << 41] = 41,
                [(long)1 << 42] = 42,
                [(long)1 << 43] = 43,
                [(long)1 << 44] = 44,
                [(long)1 << 45] = 45,
                [(long)1 << 46] = 46,
                [(long)1 << 47] = 47,
                [(long)1 << 48] = 48,
                [(long)1 << 49] = 49,
                [(long)1 << 50] = 50,
                [(long)1 << 51] = 51,
                [(long)1 << 52] = 52,
                [(long)1 << 53] = 53,
                [(long)1 << 54] = 54,
                [(long)1 << 55] = 55,
                [(long)1 << 56] = 56,
                [(long)1 << 57] = 57,
                [(long)1 << 58] = 58,
                [(long)1 << 59] = 59,
        };

        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var totalPriority = 0;
                var teamPriority = 0;
                for(int i = 0; i < lines.Length; i+=3)
                {
                        for(var j = i; j < i + 3; j++)
                        {
                                var span = lines[j].AsSpan();

                                var p1h1 = GetHash(span.Slice(0, span.Length / 2));
                                var p1h2 = GetHash(span.Slice(span.Length / 2));

                                var p1sc = p1h1 & p1h2;

                                var p1Prio = logDict[p1sc];

                                totalPriority += p1Prio;
                        }

                        var h1 = GetHash(lines[i]);
                        var h2 = GetHash(lines[i + 1]);
                        var h3 = GetHash(lines[i + 2]);

                        var sharedLetter = h1 & h2 & h3;
                        teamPriority += logDict[sharedLetter];
                }

                Console.WriteLine((totalPriority.ToString(), teamPriority.ToString()));
        }

        private long GetHash(ReadOnlySpan<char> str)
        {
                long hash = 0;
                foreach(var c in str)
                {
                        if (c >= 'a' && c <= 'z')
                        {
                                hash |= (long)1 << (1 + c - 'a');
                        }
                        else
                        {
                                hash |= (long)1 << (27 + c - 'A');
                        }
                }
                return hash;
        }
}