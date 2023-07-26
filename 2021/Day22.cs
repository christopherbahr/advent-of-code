using System.Numerics;

namespace AOC21;
public class Day22 : IDay
{
        HashSet<(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on)> cuboidMap = new HashSet<(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on)>();
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                // The root idea here is to incrementally deal with each cuboid
                // Essentially we try add each cuboid to a map, if it collides with any
                // existing cuboid we break the combined cuboids into a bunch of component
                // cuboids and record the enabled ones
                // then at the end we just need to multiply length * width * height for each cuboid in the map and sum up
                var p1 = false;
                foreach(var (i, line) in lines.Enumerate())
                {
                        var ranges = line.GetInts();
                        var dir = line.Split(' ')[0];
                        // p1
                        if(Math.Abs(ranges[0]) > 50 && !p1)
                        {
                                Console.WriteLine(CountEnabled());
                                p1 = true;
                        }
                        var cuboid = (ranges[0], ranges[1], ranges[2], ranges[3], ranges[4], ranges[5], dir == "on");
                        UpdateCuboids(cuboid);
                }

                Console.WriteLine(CountEnabled());
        }

        private BigInteger CountEnabled()
        {
                BigInteger totalCount = 0;

                foreach(var (i, c1) in cuboidMap.Enumerate())
                {
                        totalCount += (BigInteger)(c1.xmax - c1.xmin + 1) * (BigInteger)(c1.ymax - c1.ymin + 1) * (BigInteger)(c1.zmax - c1.zmin + 1);
                }

                return totalCount;
        }

        private void UpdateCuboids((int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on) cuboid)
        {
                // Find any existig cuboids that this new cuboid collides with
                var overlap = new List<(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on)>();
                foreach(var c in cuboidMap)
                {
                        var xOverlap = c.xmax >= cuboid.xmin && c.xmin <= cuboid.xmax;
                        var yOverlap = c.ymax >= cuboid.ymin && c.ymin <= cuboid.ymax;
                        var zOverlap = c.zmax >= cuboid.zmin && c.zmin <= cuboid.zmax;
                        if(xOverlap && yOverlap && zOverlap)
                        {
                                overlap.Add(c);
                        }
                }


                // If we don't overlap anything and this is an enabling cuboid just add it to the map
                if(!overlap.Any() && cuboid.on)
                {
                        cuboidMap.Add(cuboid);
                        return;
                }
                else
                {
                        // Create a new cuboid map with all the existing, non-overlapping cuboids
                        // the rest of the work is to deal with the overlapping ones.
                        var oldmap = cuboidMap;
                        var cm = new HashSet<(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on)>();
                        foreach(var c in oldmap.Except(overlap))
                        {
                                cm.Add(c);
                        }

                        foreach(var c in overlap)
                        {
                                // As a mental model we can think of a cuboid fully enveloped in another cuboid
                                // If we just extend the edges of the encompassed cuboid until the touch the encompassing cuboid
                                // those edges would cut the encompassing cuboid into 27 cuboids. 
                                // 3 layers of 9 cuboids with the centermost one being the encompassed cuboid.

                                // This algorithm essentially calculates each of those cuboids and adds them to the map
                                // unless the center cuboid is turned off in which case we just add 26

                                // We can combine a bunch of those cuboids together. Imagining those 3 layers of 9 cuboids
                                // We can combine all the cuboids of each edge layer (in any direction we want)
                                // to create a "sheet" layer. 
                                // Then we can combine the 3 cuboids on either side that make bars between those layers.
                                // Then we're left with just the 3 cuboid down the center of the overall cuboid.
                                // These need to be calculated independently. So we're left with only 7 cuboids to 
                                // calculate total.

                                // With slight modification that example turns out to be enough even though there are
                                // many ways cuboids can intersect besides one completely encompassing the other

                                // The trick is that as the "inner" cuboid starts to poke out in various directions
                                // some of the sheets or bars have negative volume. We consider those "invalid" cuboids
                                // and simply ignore them.

                                // The other complexity lies in keeping the sheets and bars from being too big when 
                                // the inner cuboid pokes out. That is handled by taking the largest min and smallest max
                                // in the dimensions that are supposed to be "within" the bounds of the cuboid.

                                var toAdd = new List<(int, int, int, int, int, int, bool)>();

                                // The algorithm for cutting a hole in something and adding something together is basically identical
                                // First you figure out how much of c is left if you cut cuboid out of it
                                // Then if it's a cut you're done. If it's an addition, just add cuboid

                                // 2 faces of the cuboid
                                toAdd.Add((c.xmin, cuboid.xmin - 1, c.ymin, c.ymax, c.zmin, c.zmax, true));
                                toAdd.Add((cuboid.xmax + 1, c.xmax, c.ymin, c.ymax, c.zmin, c.zmax, true));

                                // 2 "bars" running next to the faces
                                toAdd.Add((Math.Max(cuboid.xmin, c.xmin), Math.Min(cuboid.xmax, c.xmax), c.ymin, cuboid.ymin - 1, c.zmin, c.zmax, true));
                                toAdd.Add((Math.Max(cuboid.xmin, c.xmin), Math.Min(cuboid.xmax, c.xmax), cuboid.ymax + 1, c.ymax, c.zmin, c.zmax, true));

                                // 2 remaining cubes that sit between the bars
                                toAdd.Add((Math.Max(cuboid.xmin, c.xmin), Math.Min(cuboid.xmax, c.xmax), Math.Max(cuboid.ymin, c.ymin), Math.Min(cuboid.ymax, c.ymax), c.zmin, cuboid.zmin - 1, true));
                                toAdd.Add((Math.Max(cuboid.xmin, c.xmin), Math.Min(cuboid.xmax, c.xmax), Math.Max(cuboid.ymin, c.ymin), Math.Min(cuboid.ymax, c.ymax), cuboid.zmax + 1, c.zmax, true));
                                
                                if(cuboid.on)
                                {
                                        toAdd.Add(cuboid);
                                }

                                foreach(var cta in toAdd.Where(x => Validate(x)))
                                {
                                        cm.Add(cta);
                                }
                        }
                        cuboidMap = cm;
                }
        }

        public bool Validate((int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool on) cuboid)
        {
                return cuboid.xmin <= cuboid.xmax && cuboid.ymin <= cuboid.ymax && cuboid.zmin <= cuboid.zmax;
        }

}