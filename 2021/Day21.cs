using System.Numerics;

namespace AOC21;
public class Day21 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var gameState = new List<(int, int)>();
                foreach(var line in lines)
                {
                        gameState.Add((line.GetInts()[1] - 1, 0));
                }

                var rollCounter = 0;
                var determDieCounter = 1;
                var done = false;
                while(!done)
                {
                        var ngs = new List<(int, int)>();
                        foreach(var (i, (p, s)) in gameState.Enumerate())
                        {
                                var pi = determDieCounter;
                                determDieCounter = (determDieCounter + 1) % 100;
                                pi += determDieCounter;
                                determDieCounter = (determDieCounter + 1) % 100;
                                pi += determDieCounter;
                                determDieCounter = (determDieCounter + 1) % 100;

                                rollCounter += 3;

                                var np = (p + pi) % 10;
                                var ns = s + np + 1;
                                ngs.Add((np, ns));

                                if(ns >= 1000)
                                {
                                        done = true;
                                        break;
                                }
                        }
                        if(!done)
                        {
                                gameState = ngs;
                        }
                }

                Console.WriteLine($"{rollCounter} {gameState.First()} {gameState.Skip(1).First()}");

                Console.WriteLine(rollCounter * gameState.Min(x => x.Item2));

                // refresh game state
                // This feels like a dynamic programming problem..
                // Let's give that a shot

                var p1 = lines.First().GetInts()[1] - 1;

                var p2 = lines.Skip(1).First().GetInts()[1] - 1;

                var (w1, w2) = SimulateQuantumDirac(p1, 0, p2, 0, 0, true, 3);
                var mostWins = w1 > w2 ? w1 : w2;
                Console.WriteLine(mostWins);

        }

        // game state is an int, first 4 bits says who's turn it is and how many rolls left
        //, next 7 hold position of player 1, next 8 are score of player 1
        // next 8 are position of player 2, next 8 are score of player 2
        private Dictionary<int, (BigInteger, BigInteger)>  cache = new Dictionary<int, (BigInteger, BigInteger)>();

        // Note: may be worth trying to generate "reverse" state where variables & outcomes are swapped
        // Note: may be worth simulating whole turns. There are 27 possible rolls of the 3 sided die but
        // many of them are equivalent since you sum them up (ie 1, 2, 3 is the same as 3, 2, 1 and 2, 2, 2)
        // Result: This is actually very fast as is. I guess no need
        private (BigInteger, BigInteger) SimulateQuantumDirac(int p1, int s1, int p2, int s2, int accumulatedRolls, bool p1Turn, int rollsLeft)
        {
                // bit pack the key for fast reads
                int ck = p1Turn ? 1 : 0; // uses bit 0
                ck += rollsLeft << 1; // uses bit 1, 2
                ck += p1 << 3; // This maxes at 10 so uses 3, 4, 5, 6
                ck += s1 << 7; // Maxes at 21 so needs 5 bits
                ck += p2 << 12; // 
                ck += s2 << 16;
                ck += accumulatedRolls << 21;
                if(cache.ContainsKey(ck))
                {
                        return cache[ck];
                }

                BigInteger w1 = 0;
                BigInteger w2 = 0;

                for(var i = 1; i < 4; i++)
                {
                        var roll = accumulatedRolls + i;
                        if(rollsLeft > 1)
                        {
                                var (nw1, nw2) = SimulateQuantumDirac(p1, s1, p2, s2, roll,p1Turn, rollsLeft - 1);
                                w1 += nw1;
                                w2 += nw2;
                        }
                        else
                        {
                                var pi = roll;
                                var p1Next = !p1Turn;
                                var np1 = p1;
                                var ns1 = s1;
                                var np2 = p2;
                                var ns2 = s2;
                                var dw = p1Turn ? (1, 0) : (0, 1);

                                if (p1Turn)
                                {
                                        np1 = (p1 + pi) % 10;
                                        ns1 = s1 + np1 + 1;
                                }
                                else
                                {
                                        np2 = (p2 + pi) % 10;
                                        ns2 = s2 + np2 + 1;
                                }

                                if (ns1 >= 21)
                                {
                                        w1++;
                                        continue;
                                }
                                else if(ns2 >= 21)
                                {
                                        w2++;
                                        continue;
                                }

                                var (nw1, nw2) = SimulateQuantumDirac(np1, ns1, np2, ns2, 0, p1Next, 3);
                                w1 += nw1;
                                w2 += nw2;
                        }
                }

                //Console.WriteLine((w1, w2));
                cache[ck] = (w1, w2);
                return (w1, w2);
        }
}