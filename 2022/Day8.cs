// Camp Cleanup
namespace AOC22;
public class Day8 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var width = lines.First().Length;
                var height = lines.Count();
                int [,] treeHeight = new int[width,height];
                bool [,] treeVis = new bool[width,height];
                int [,] treeScore = new int[width,height];

/*
                foreach(var i in Enumerable.Range(0, height))
                {
                        foreach(var j in Enumerable.Range(0, width))
                        {
                                treeHeight[i,j] = int.Parse(lines[i][j].ToString());
                                Console.Write(treeHeight[i,j]);
                        }
                        Console.WriteLine();
                }


                var dir = new List<(int, int)>{ (-1, 0), (1, 0), (0, 1), (0, -1)};

                var shortVCount = 0;
                foreach(var i in Enumerable.Range(0, height))
                {
                        foreach(var j in Enumerable.Range(0, width))
                        {
                                var ch = treeHeight[i, j];
                                var vis = false;
                                var ci = i;
                                var cj = j;
                                foreach (var (di, dj) in dir)
                                {
                                        var canSee = true;
                                        while(true)
                                        {
                                                ci += di;
                                                cj += dj;
                                                if(ci < 0 || ci >= height || cj < 0 || cj >= height)
                                                {
                                                        break;
                                                }

                                                if (treeHeight[ci, cj] > ch)
                                                {
                                                        canSee = false;
                                                }
                                        }
                                        if (canSee)
                                        {
                                                vis = true;
                                        }
                                }
                                if(vis)
                                {
                                        shortVCount++;

                                }
                        }

                }
*/
                for(var j = 0; j < height; j++)
                {
                        for(int i = 0; i< width; i++)
                        {
                                treeHeight[i,j] = int.Parse(lines[j][i].ToString());
                        }
                }

                // outer ring of trees is always visible
                var outer = width * 2 + height * 2 - 4;


                // check by each direction first
                // check visibility from top
                var highestSoFar = -1;
                for(int i = 0; i < width; i++)
                {
                        for(int j = 0; j < height; j++)
                        {
                                // a tree is visible if it is taller than the predeccessor and we haven't stopped yet
                                if(treeHeight[i, j] > highestSoFar)
                                {
                                        treeVis[i, j] = true;
                                        highestSoFar = treeHeight[i,j];
                                }
                        }
                highestSoFar = -1;
                }

                highestSoFar = -1;

                // now bottom
                for(int i = 0; i < width; i++)
                {
                        for(int j = height - 1; j >= 0; j--)
                        {
                                // a tree is visible if it is taller than the predeccessor and we haven't stopped yet
                                if(treeHeight[i, j] > highestSoFar)
                                {
                                        treeVis[i, j] = true;
                                        highestSoFar = treeHeight[i,j];
                                }
                        }
                        highestSoFar = -1;
                }
                highestSoFar = -1;

                // now left
                for (int j = 0; j < height; j++)
                {
                        for (int i = 0; i < width; i++)
                        {
                                // a tree is visible if it is taller than the predeccessor and we haven't stopped yet
                                if (treeHeight[i, j] > highestSoFar)
                                {
                                        treeVis[i, j] = true;
                                        highestSoFar = treeHeight[i, j];
                                }
                        }
                highestSoFar = -1;
                }
                highestSoFar = -1;

                // finally right
                for (int j = 0; j < height; j++)
                {
                        for (int i = width-1; i >= 0; i--)
                        {
                                // a tree is visible if it is taller than the predeccessor and we haven't stopped yet
                                if (treeHeight[i, j] > highestSoFar)
                                {
                                        treeVis[i, j] = true;
                                        highestSoFar = treeHeight[i, j];
                                }
                        }
                highestSoFar = -1;
                }

                var counter = 0;

                for (int j = 0; j < height; j++)
                {
                        for (int i = 0; i < width; i++)
                        {
                                Console.Write(treeHeight[i,j]);
                        }
                        Console.WriteLine();
                }

                for (int j = 0; j < height; j++)
                {
                        for (int i = 0; i < width; i++)
                        {
                                Console.Write(treeVis[i,j] ? "T" : "F");
                                if(treeVis[i,j])
                                {
                                         counter++;
                                }
                        }
                        Console.WriteLine();
                }


                var visCount = counter;

                // Now we need to find the tree that can see the furthest

                var maxSceneScore = 0;
                for(int i = 0; i < height; i++)
                {
                        for(int j = 0; j < width; j++)
                        {
                                var visCounter = 0;
                                var score = 1;
                                var candidateHeight = treeHeight[i,j];
                                for(int ii = i + 1; ii < height; ii++)
                                {
                                        visCounter++;
                                        if(treeHeight[ii,j] >= candidateHeight)
                                        {
                                                break;
                                        }
                                }
                                Console.WriteLine($"For [{i},{j}] found {visCounter} trees looking down");
                                score *= visCounter;

                                visCounter = 0;
                                for(int ii = i - 1; ii >= 0; ii--)
                                {
                                        visCounter++;
                                        if(treeHeight[ii,j] >= candidateHeight)
                                        {
                                                break;
                                        }
                                }
                                Console.WriteLine($"For [{i},{j}] found {visCounter} trees looking up");
                                score *= visCounter;

                                visCounter = 0;

                                for(int jj = j + 1; jj < width; jj++)
                                {
                                        visCounter++;
                                        if(treeHeight[i,jj] >= candidateHeight)
                                        {
                                                break;
                                        }
                                }
                                Console.WriteLine($"For [{i},{j}] found {visCounter} trees looking right");
                                score *= visCounter;

                                visCounter = 0;

                                for(int jj = j - 1; jj >= 0; jj--)
                                {
                                        visCounter++;
                                        if(treeHeight[i,jj] >= candidateHeight)
                                        {
                                                break;
                                        }
                                }
                                Console.WriteLine($"For [{i},{j}] found {visCounter} trees looking left");
                                score *= visCounter;

                                treeScore[i,j] = score;
                                Console.WriteLine($"For [{i},{j}] got {score} scene score");
                                if(score > maxSceneScore)
                                {
                                        maxSceneScore = score;
                                }
                        }
                }

                Console.WriteLine( (visCount.ToString(), maxSceneScore.ToString()));
        }
}