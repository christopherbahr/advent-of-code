using System.Diagnostics;
using System.Numerics;

// The first working solution of this took 80-84 seconds to run
// Ideas for improvement:
// 1. 
// We currently figure out the correct rotation by trying them all and waiting until we
// find one that satisfies leads to a consistent translation vector for multiple beacons.
// That's dumb because we already have the transformation we needed to get from one coordinate
// space to the other when we detected the overlap.
// Something is wrong with that code though. Although it finds a consistent transform that transform
// doesn't work in all cases. There does seem to be a pattern though, it's not random. We need to 
// investigate what the connection is between the pairs of transforms that it thinks we should use vs
// the ones we actually should use are. Notably some of them actually work? :shrug:
//
// Result: I don't know why some of them used to work but the problem was just that the transform
// and the overlap that detected the transform were going in opposite directions.
// The detected tranform was how to transform p1 to p2 and then we were applying that to p2.
// The ones that worked must be symmetric somehow although I can't get my brain around why.
// It's also notable that to detect vectors going both ways we have to consider both the
// negative and positive vectors but we aggregate the transforms (because it's really one transform
// we may have just guessed wrong about initial vector direction). So.. more weirdness three maybe? 
// That may also mean that enumerating possible transforms was dumb and we should have just contemplated
// all of them.
// Anyway switching the order in the overlap detection and removing the transform discovery loop
// got the time down to 75s. Not a huge gain but this was an obviously broken part of the code so it's nice to fix it.
// 2.
// Overlap detection currently compares every pair of beacons to every other pair of beacons
// between two sensors. To compare 2 beacon pairs it calculates the vector between a pair and then
// checks if any of the 24 transformations make the 2 vectors match. 
// This could be improved dramatically in several ways. Firstly we pre-calculate a list of distances
// between each beacon in a given sensor. Then we could only compare sensors that have at least 12 shared distances.
// Continuing with that we could only compare vectors whose manhattan distance is the same.
// Once we get to finding transforms we could calculate one parameter at a time rather than checking all 24 
// ie: figure out where x should go, figure out where y should go, figure out where z should go
// We'd have to ensure that the transformation was viable but we could do that by comparing to a hash set of 
// characteristic transformations somehow. (basically 0 = x, 1 = -x, 2 = y, 3 = -y, 4 = z, 5 = -z) transorms = (0, 2, 4)...
//
// Result: Just adding a manhattan distance checker before the vector congruency checker reduced runtime to 7s.
// I also tried sorting the components of each vector and making sure they were the same but that was slower than just checking distance
// We can still do better by pre-computing distance for each pair af beacons within a sensor and then iterating sorted lists of vectors
// to check (and maybe to filter out entire sensors who don't have overlapping distances?)
//
// Result: Pre-Computing distances for each scanner into sorted lists and then deeply checking only scanners who had at least
// 66 overlapping distances (12 shared beacons is 12(12 - 1)/2 = 66 shared edges) brought this down to under a second.
// Essentially this cheaply filters out sensors that don't overlap but doesn't calculate their transforms or anything
// this could be merged with the regular beacon enumeration that does detect transforms and it would be even faster.
//
// Result: Capturing which pains match to which distance and then only comparing relevant pairs of beacons when determining
// scanner overlap brought the time down under 100ms.
// I think that's as well as we're going to do for low hanging fruit.
// 3.
// We actually don't need to normalize every beacon, we only need to know the overlap count & the scanner transform & vector
// That may not save that much time though. Normalization itself should be pretty fast.
// 4.
// The Vector3 class that I'm using uses all floats which is annoying because int would be fine for the scope of this problem
// (actually short would also be fine but I tried and it doesn't work because short + short wants to give an int becuase that how c# works.
// The internet tells me that short math is likely slower anyway since everything is optimized for ints);
// Result:
// The one built into .NET is faster than a simple hand rolled int one even though it uses floats. SIMD is magic.
// maybe it's the same anyway, they're both 4 bytes wide and I'm just doing addition and subtraction
// it shouldn't need any of the float magic
//

namespace AOC21;
public class Day19 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public Dictionary<int, Dictionary<(int, int), (float, Vector3)>> distDict = new Dictionary<int, Dictionary<(int, int), (float, Vector3)>>();
        public void Execute()
        {
                var timer = Stopwatch.StartNew();
                var lines = File.ReadAllLines(inputFile);

                var scanners = new List<List<Vector3>>();
                var scanner = new List<Vector3>();
                
                foreach(var line in lines)
                {
                        if(line.StartsWith("---"))
                        {
                                continue;
                        }
                        if(string.IsNullOrEmpty(line))
                        {
                                scanners.Add(scanner);
                                scanner = new List<Vector3>();
                        }
                        else
                        {
                                var data = line.Split(',').Select(int.Parse).ToList();
                                var newVec = new Vector3(data[0], data[1], data[2]);
                                scanner.Add(newVec);
                                //Console.WriteLine(newVec);
                        }
                }
                if(scanner.Any() && !scanners.Contains(scanner))
                {
                        scanners.Add(scanner);
                }

                // Okay so the plan here is basically a BFS across scanners
                // starting from Scanner 0
                // We check all the other scanners to see if they overlap 0
                // If they do then we normalize them to Scanner 0's coordinate system
                // add them to the queue and visit them next.
                // In turn they check all non-normalized scanners and normalize any
                // that they overlap with

                var scannerPos = new List<Vector3>();
                var toNormalize = new HashSet<int>();
                var normQueue = new Queue<int>();
                var normScanners = new Dictionary<int, List<Vector3>>();
                var allBeacons = new HashSet<Vector3>();

                foreach(var (i, s) in scanners.Enumerate())
                {
                        DistanceCharacteristics.Add(GetScannerDistances(s, i));
                        toNormalize.Add(i);
                }
                // scanner 0 is already normalized. Add all of its beacons
                foreach(var b in scanners[0])
                {
                        allBeacons.Add(b);
                }
                toNormalize.Remove(0);

                normQueue.Enqueue(0);
                normScanners.Add(0, scanners[0]);
                while(normQueue.Any())
                {
                        var si = normQueue.Dequeue();
                        var s = normScanners[si];
                        foreach (var (i, s1) in scanners.Enumerate())
                        {
                                if(!toNormalize.Contains(i))
                                {
                                        continue;
                                }

                                var (p, map, tform) = ScannersOverlap(si, s, i, s1);
                                // We found some connection from a normalized sensor
                                // to an un-normalized vector
                                // Now we can normalize it
                                if(p)
                                {
                                        Vector3? pv = null;
                                        var p1 = s[map.First().Key];
                                        // rotate by detected transform
                                        var p2 = GetTransform(s1[map.First().Value], tform);
                                        // translate after rotation
                                        pv = p1 - p2;
                                        var ns = new List<Vector3>();
                                        foreach(var b in s1)
                                        {
                                                var nb = GetTransform(b, tform);
                                                nb = nb + pv.Value;
                                                ns.Add(nb);
                                                allBeacons.Add(nb);
                                        }
                                        normScanners[i] = ns;
                                        normQueue.Enqueue(i);
                                        toNormalize.Remove(i);
                                        scannerPos.Add(pv.Value);
                                        //Console.WriteLine($"Normalized {i} {pv} {tform}");
                                }

                        }
                }

                Console.WriteLine(allBeacons.Count());
                var maxDist = 0f;
                foreach(var b1 in scannerPos)
                {
                        //Console.WriteLine(b1);
                        foreach(var b2 in scannerPos)
                        {
                                if(b1 == b2)
                                { 
                                        continue;
                                }
                                var dist = Math.Abs(b1.X - b2.X) + Math.Abs(b1.Y - b2.Y) + Math.Abs(b1.Z - b2.Z);
                                maxDist = Math.Max(maxDist, dist);
                        }
                }
                Console.WriteLine(maxDist);
                Console.WriteLine("=== TIMING ====");
                Console.WriteLine(timer.ElapsedMilliseconds);
                Console.WriteLine(counter);
                Console.WriteLine("=== TIMING ====");
        }

        private List<(float, int, int)> GetScannerDistances(List<Vector3> s, int idx)
        {
                var distList = new List<(float, int, int)>();
                foreach(var (k, b1) in s.Enumerate())
                {
                        foreach(var (l, b2) in s.Enumerate())
                        {
                                if(l <= k)
                                {
                                        continue;
                                }
                                var tv = b2 - b1;
                                var md = Math.Abs(tv.X) + Math.Abs(tv.Y) + Math.Abs(tv.Z);
                                distList.Add((md, k, l));
                        }
                }
                return distList.OrderBy(x => x.Item1).ToList();
        }

        private long counter = 0;
        
        private List<List<(float, int, int)>> DistanceCharacteristics = new List<List<(float, int, int)>>();

        private (bool, List<((int, int), (int, int))>) QuickOverlapCheck(int s1Idx, int s2Idx)
        {
                var l1 = DistanceCharacteristics[s1Idx];
                var l2 = DistanceCharacteristics[s2Idx];
                var overlap = 0;
                var pairs = new List<((int, int), (int, int))>();
                for(var (i,j) = (0, 0); (i < l1.Count) && (j < l2.Count);)
                {
                        // found an overlap
                        if (l1[i].Item1 == l2[j].Item1)
                        {
                                overlap++;
                                var pair = ((l1[i].Item2, l1[i].Item3), (l2[j].Item2, l2[j].Item3));
                                pairs.Add(pair);
                                i++;
                                j++;
                        }
                        else if (l1[i].Item1 < l2[j].Item1)
                        {
                                // l1 is lower, increment its pointer
                                i++;
                        }
                        else
                        {
                                // l2 is lower, increment its pointer
                                j++;
                        }
                }
                // If there are 12 shared scanners
                // there will be n(n-1)/2 shared distances 
                return (overlap >= 66, pairs);
        }

        private (bool p, Dictionary<int, int> m, int tformIdx) ScannersOverlap(int s1Idx, List<Vector3> s1, int s2Idx, List<Vector3> s2)
        {
                var (overlap, pairs) = QuickOverlapCheck(s1Idx, s2Idx);
                if(!overlap)
                {
                        return (false, null, -1);
                }
                counter++;
                var possibleMappings = new Dictionary<int, Dictionary<int, int>>();

                var tForms = new Dictionary<int, int>();
                var d = new Dictionary<int, int>();
                foreach(var ((k, l), (m,n)) in pairs)
                {
                        var b1 = s1[k];
                        var b2 = s1[l];
                        var b3 = s2[m];
                        var b4 = s2[n];

                        var tv1 = b2 - b1;
                        var md1 = Math.Abs(tv1.X) + Math.Abs(tv1.Y) + Math.Abs(tv1.Z);
                        var tv2 = b4 - b3;
                        var md2 = Math.Abs(tv2.X) + Math.Abs(tv2.Y) + Math.Abs(tv2.Z);
                        var (p, o) = PossiblyCongruent(tv2, tv1);

                        // The implication of congruency is that 
                        // (b2 = b4, b1 = b3) || (b2 = b3, b1 = b4)
                        // to disambiguate we need to build up viable mappings
                        if (p)
                        {
                                tForms.AddToVal(o, 1);
                                if (possibleMappings.ContainsKey(k))
                                {
                                        var dict = possibleMappings[k];
                                        dict.AddToVal(m, 1);
                                        dict.AddToVal(n, 1);
                                }
                                else
                                {
                                        possibleMappings[k] = new Dictionary<int, int>();
                                        possibleMappings[k][m] = 1;
                                        possibleMappings[k][n] = 1;
                                }

                                if (possibleMappings.ContainsKey(l))
                                {
                                        var dict = possibleMappings[l];
                                        dict.AddToVal(m, 1);
                                        dict.AddToVal(n, 1);
                                }
                                else
                                {
                                        possibleMappings[l] = new Dictionary<int, int>();
                                        possibleMappings[l][m] = 1;
                                        possibleMappings[l][n] = 1;
                                }
                        }

                }

                var mappings = new Dictionary<int, int>();
                foreach(var entry in possibleMappings)
                {
                        var ans = entry.Value.MaxBy(x => x.Value);
                        mappings[entry.Key] = ans.Key;
                }

                if(mappings.Count < 12)
                {
                        return (false, null, -1);
                }
                else
                {
                        return (true, mappings, tForms.MaxBy(x => x.Value).Key);
                }

        }

        // There are 24 possible "orientations" of a scanner
        // 4 rotations about an axis * 2 possible directions to point on an axis * 3 axes
        //
        // They manifest like this for a point like (1, 2, 3)
        // rotation about x.
        // (1, 2, 3)
        // (1, -3, 2)
        // (1, 3, -2)
        // (1, -2, -3)
        // (-1, 2, 3)
        // (-1, -3, 2)
        // (-1, 3, -2)
        // (-1, -2, -3)


        // So to see if 2 sets of 2 points (x1, y1, z1) (x2, y2, z2), (x1', x1', x1') (x2', x2', x2')
        // are possibly the same we can take the differences (x1 - x2, y1 - y2, z1 - z2), (x1' - x2', y1' - y2', z1' - z2')
        // and see if the two vectors are transformable into the other by a rotation

// This list of transforms is wrong :()
        private Vector3 GetTransform(Vector3 v, int t)
        {
                return GetPossibleTransforms(v)[t];
        }

        private Stopwatch sw = new Stopwatch();

        private List<Vector3> GetPossibleTransforms(Vector3 v)
        {
                var (x, y, z) = (v.X, v.Y, v.Z);
                // x rotation & flip
                // One way to reason about this is to imagine that you have a cube
                // a cube has 6 faces each of which has 4 cardinal directions
                // you can think that your sensor is pointed at a particular face 
                // of this cube and in a particular cardinal direction
                // Call the coordinate (x, y, z) the coordinate that you start with
                // Imagine that your sensor is pointed along the +ve x axis.
                // rotate in the 4 cardinal directions to get 4 possible coordinates.

                // Now rotate 90 degrees about y and repeat rotation about x
                // do that 2 more times. Now you've covered all the "side" faces of the cube
                // now rotate 90 degrees about z so you're pointing up & repeat
                // now rotate 180 degrees about y or z and repeat again

                var tform = new[]
                {
                        // rotate about x
                        (x, y, z),
                        (x, z, -y),
                        (x, -z, y),
                        (x, -y, -z),
                        // rotate 90 degrees about y
                        // and then repeat rotation about "first" coordinate (x)
                        (z, y, -x),
                        (z, x, y),
                        (z, -x, -y), // 6
                        (z, -y, x),
                        // rotate 90 degrees about y again
                        // and then repeat rotation about first coordinate
                        (-x, y, -z),
                        (-x, z, y),
                        (-x, -z, -y),
                        (-x, -y, z),
                        // one more time
                        (-z, y, x),
                        (-z, -x, y),
                        (-z, x, -y),
                        (-z, -y, -x),

                        // Now do north & south
                        // Starting from original position (x, y, z) rotate
                        // 90 degrees about z and repeat rotation about first coord
                        (-y, x, z),
                        (-y, -z, x), //17
                        (-y, z, -x),
                        (-y, -x, -z),

                        // Now starting from original position (x, y, z) rotate 
                        // 90 degrees about z the other way
                        (y, -x, z),
                        (y, z, x),
                        (y, -z, -x),
                        (y, x, -z)
                }.Select(x => new Vector3(x.Item1, x.Item2, x.Item3));
                return tform.ToList();
        }

        private (bool possible, int tformIdx) PossiblyCongruent(Vector3 v1, Vector3 v2)
        {
                var v1tforms = GetPossibleTransforms(v1);
                var idx = v1tforms.IndexOf(v2);
                if(idx >= 0)
                {
                        return (true, idx);
                }
                else
                {
                        idx = v1tforms.IndexOf(-v2);
                        if(idx >= 0)
                        {
                                return (true, idx);
                        }
                        else
                        {
                                return (false, -1);
                        }
                }
        }
}