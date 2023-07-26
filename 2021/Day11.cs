namespace AOC21;
public class Day11 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var grid = new int[10,10];

                foreach(var ((r,c), power) in lines.IntGrid())
                {
                        grid[r, c] = power;
                }

                var flashCount = 0;

                var counter = 0;
                while(true)
                {
                        var flashed = new HashSet<(int, int)>();
                        foreach(var ((r,c), v) in grid.Enumerate())
                        {
                                grid[r,c] += 1;
                        }

                        var anyFlashed = true;
                        while(anyFlashed)
                        {
                                anyFlashed = false;
                                foreach(var ((r,c), v) in grid.Enumerate())
                                {
                                        if (v > 9)
                                        {
                                                if (flashed.Add((r, c)))
                                                {
                                                        flashCount++;
                                                        anyFlashed = true;
                                                        foreach(var (cr, cc) in (r, c).allDirs())
                                                        {
                                                                if(grid.ContainsPoint(cr, cc))
                                                                {
                                                                        grid[cr, cc] += 1;
                                                                }
                                                        }
                                                }
                                        }
                                }
                        }
                        foreach(var (fr, fc) in flashed)
                        {
                                grid[fr, fc] = 0;
                        }
                        //Console.WriteLine($"{i} - {flashCount}");
                        //PrettyPrint(grid);
                        counter++;
                        if(counter == 100)
                        {
                                Console.WriteLine(flashCount);
                        }
                        if(flashed.Count() == 100)
                        {
                                Console.WriteLine(counter);
                                break;
                        }
                }
        }
}