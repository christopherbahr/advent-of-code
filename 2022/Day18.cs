// Camp Cleanup
namespace AOC22;
public class Day18 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var cubes = new int[30,30,30];


                foreach(var line in lines)
                {
                        var coordinates = line.Split(',').Select(int.Parse).ToArray();
                        var (x, y, z) = (coordinates[0] + 5, coordinates[1] + 5, coordinates[2] + 5);
                        cubes[x, y, z] = 1;
                }

                var sides = 0;

                var dirs = new[] { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1)};

                var possiblePoints = new HashSet<(int, int, int)>();
                for(int x = 0; x < 30; x++)
                {
                        for (int y = 0; y < 30; y++)
                        {
                                for (int z = 0; z < 30; z++)
                                {
                                        // do we have a cube?
                                        if(cubes[x,y,z] == 1)
                                        {
                                                foreach(var (dx, dy, dz) in dirs)
                                                {
                                                        if(cubes[x + dx, y + dy, z + dz] != 1)
                                                        {
                                                                sides++;
                                                                possiblePoints.Add((x + dx, y + dy, z + dz));
                                                        }
                                                }

                                        }

                                }
                        }
                }

                Console.WriteLine($"Considering {possiblePoints.Count()} border cubes");

                // all empty cubes that border populated cubes
                var nonEscape = new HashSet<(int, int, int)>();
                var escape = new HashSet<(int, int, int)>();
                while(true)
                {
                        // we care about border points which so far are neither marked as escaped on non escaped
                        var relevantSet = possiblePoints.Except(nonEscape).Except(escape);
                        if(!relevantSet.Any())
                        {
                                break;
                        }
                        var (x, y, z) = relevantSet.First();

                        var (canEscape, set) = CanEscape(x, y, z, cubes, new HashSet<(int, int, int)>());

                        if(canEscape)
                        {
                                foreach(var c in set)
                                {
                                        escape.Add(c);
                                }
                        }
                        else
                        {
                                foreach(var c in set)
                                {
                                        nonEscape.Add(c);
                                }
                        }
                }

                var escapePoints = possiblePoints.Except(nonEscape);
                var outerSides = 0;
                foreach(var (x, y, z) in escapePoints)
                {
                        var escapeSides = 0;
                        foreach (var (dx, dy, dz) in dirs)
                        {
                                if (cubes[x + dx, y + dy, z + dz] == 1)
                                {
                                        escapeSides++;
                                }
                        }
                        outerSides += escapeSides;
                }

                Console.WriteLine(sides);

                Console.WriteLine(outerSides);
                
                Console.WriteLine( (sides.ToString(), outerSides.ToString()));
        }

        private (bool, HashSet<(int, int, int)>) CanEscape(int x, int y, int z, int[,,] cubes, HashSet<(int, int, int)> neighbors)
        {
                var escaped = false;
                var dirs = new[] { (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1)};

                neighbors.Add((x, y, z));

                if(x == 29 || y == 29 || z == 29 || x == 0 || y == 0 || z == 0)
                {
                        return (true, neighbors);
                }
                else
                {
                        foreach (var (dx, dy, dz) in dirs)
                        {
                                if (cubes[x + dx, y + dy, z + dz] != 1 && !neighbors.Contains((x + dx, y + dy, z + dz)))
                                {
                                        var (nE, nS) = CanEscape(x + dx, y + dy, z + dz, cubes, neighbors);
                                        escaped |= nE;
                                        foreach(var c in nS)
                                        {
                                                neighbors.Add(c);
                                        }
                                        if(escaped)
                                        {
                                                return (escaped, neighbors);
                                        }
                                }
                        }

                }

                return (false, neighbors);
        }
}