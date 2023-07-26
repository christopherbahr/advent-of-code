// Rock Paper Scissors
namespace AOC22;
public class Day2 : IDay
{
        public string inputFile { get; set; } = "wip.in";

        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var partOneScore = 0;
                var partTwoScore = 0;
                foreach (var line in lines)
                {
                        var moves = line.Split(' ');
                        if(moves.Length != 2)
                        {
                               throw new Exception($"Malformed Input: {line}");
                        }

                        var movePair = (moves[1], moves[0]);
                        var partOneMoveScore = 0;
                        var partTwoMoveScore = 0;

                        var rockPoints = 1;
                        var paperPoints = 2;
                        var scissorsPoints = 3;

                        var lossPoints = 0;
                        var drawPoints = 3;
                        var winPoints = 6;
                        
                        // Case 1: X means Rock, Y means Paper, Z means scissors
                        // Case 2: X means lose, Y means draw, Z means win
                        switch (movePair)
                        {
                                // P1: Rock Rock
                                // P2: Lose vs Rock => Scissors Rock
                                case ("X","A"):
                                        partOneMoveScore = rockPoints + drawPoints;
                                        partTwoMoveScore = scissorsPoints + lossPoints;
                                        break;
                                // P1: Rock Paper
                                // P2: Lose vs Paper => Rock Paper
                                case ("X", "B"):
                                        partOneMoveScore = rockPoints + lossPoints;
                                        partTwoMoveScore = rockPoints + lossPoints;
                                        break;
                                // P1: Rock Scissors
                                // P2: Lose vs Scissors => Paper Scissors
                                case ("X", "C"):
                                        partOneMoveScore = rockPoints + winPoints;
                                        partTwoMoveScore = paperPoints + lossPoints;
                                        break;
                                // P1: Paper Rock
                                // P2: Draw vs Rock => Rock Rock
                                case ("Y","A"):
                                        partOneMoveScore = paperPoints + winPoints;
                                        partTwoMoveScore = rockPoints + drawPoints;
                                        break;
                                // P1: Paper Paper
                                // P2: Draw vs Paper => Paper Paper
                                case ("Y", "B"):
                                        partOneMoveScore = paperPoints + drawPoints;
                                        partTwoMoveScore = paperPoints + drawPoints;
                                        break;
                                // P1: Paper Scissors
                                // P2: Draw vs Scissors => Scissors Scissors
                                case ("Y", "C"):
                                        partOneMoveScore = paperPoints + lossPoints;
                                        partTwoMoveScore = scissorsPoints + drawPoints;
                                        break;
                                // P1: Scissors Rock
                                // P2: Win vs Rock => Paper Rock
                                case ("Z","A"):
                                        partOneMoveScore = scissorsPoints + lossPoints;
                                        partTwoMoveScore = paperPoints + winPoints;
                                        break;
                                // P1: Scissors Paper
                                // P2: Win vs Paper => Scissors Paper
                                case ("Z", "B"):
                                        partOneMoveScore = scissorsPoints + winPoints;
                                        partTwoMoveScore = scissorsPoints + winPoints;
                                        break;
                                // P1: Scissors Scissors
                                // P2: Win vs Scissors => Rock Scissors
                                case ("Z", "C"):
                                        partOneMoveScore = scissorsPoints + drawPoints;
                                        partTwoMoveScore = rockPoints + winPoints;
                                        break;
                                default:
                                        throw new Exception($"Invalid Character: {line}");
                        }
                        partOneScore += partOneMoveScore;
                        partTwoScore += partTwoMoveScore;
                }
                
                Console.WriteLine((partOneScore.ToString(), partTwoScore.ToString()));
        }
}