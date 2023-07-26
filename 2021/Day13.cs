namespace AOC21;
public class Day13 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var originalPoints = new HashSet<(int x, int y)>();
                var folds = new List<(char d, int v)>();
                foreach (var line in lines)
                {
                        if(line.StartsWith("fold"))
                        {
                                var data = line.Split('=');
                                var val = int.Parse(data[1]);
                                var ch = data[0].Split(' ').Last().Single();
                                folds.Add((ch, val));
                        }
                        else
                        {
                                if(string.IsNullOrEmpty(line))
                                {
                                        continue;
                                }
                                var data = line.Split(',').Select(int.Parse).ToList();
                                originalPoints.Add((data[0], data[1]));
                        }
                }

                var workingSet = originalPoints;
                foreach(var f in folds)
                {
                        var newSet = new HashSet<(int, int)>();
                        var (dx, dy) = f.d == 'x' ? (2, 0) : (0,2);
                        foreach (var (x, y) in workingSet)
                        {
                                var tv = f.d == 'x' ? x : y;
                                if(tv < f.v)
                                {
                                        newSet.Add((x, y));
                                }
                                else
                                {
                                        var newY = Math.Abs(dy*f.v - y);
                                        var newX = Math.Abs(dx*f.v - x);
                                        newSet.Add((newX, newY));
                                }
                        }

                        if(f == folds.First())
                        {
                                Console.WriteLine(newSet.Count());
                        }

                        workingSet = newSet;
                }

                var maxX = workingSet.Max(x => x.x);
                var maxY = workingSet.Max(x => x.y);

                for(var i = 0; i <= maxY; i++)
                {
                        var line = "";
                        for (var j = 0; j <= maxX; j++)
                        {
                                line += workingSet.Contains((j,i)) ? "#" : ".";
                        }
                        Console.WriteLine(line);
                }

        }
}