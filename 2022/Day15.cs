using System.Diagnostics;
using System.Numerics;

// Note: My original sucessful implementation was built on holding a set of "excluded ranges" for each row
// Then I would move through the target row jumping across excladed ranges looking for a point that wos not excluded
// That worked but it took about 6 seconds to build up the excluded ranges

// The idea to walk the edge of beacon range was stolen from the internet after the fact
// This core idea can be improved further. Rather than checkng every point on the edge for every sensor
// We should instead check for pairs of sensors that have a gap of 1 between them
// Once we've found that we shoud check only points from those sensors

namespace AOC22;
public class Day15 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                
                var timer = Stopwatch.StartNew();
                var lines = File.ReadAllLines(inputFile);

                BigInteger p1 = 0;
                BigInteger p2 = 0;

                var sensorBeaconPairs = new List<(int sc, int sr, int bc, int br, int range)>();

                foreach(var line in lines)
                {
                        var words = line.Split(' ').Where(x => x.StartsWith('x') || x.StartsWith('y')).Select(x=> x.Trim(':').Trim(','));

                        var data = words.Select(x => int.Parse(x.Split('=')[1])).ToList();

                        var distance = Math.Abs(data[1] - data[3]) + Math.Abs(data[0] - data[2]);
                        var sbp = (data[0], data[1], data[2], data[3], distance);

                        sensorBeaconPairs.Add(sbp);
                }

                var excludedRanges = new Dictionary<int, List<(int sc, int ec)>>();
                var rtr = 2000000;
                var ttr = 10;
                var tr = rtr;

                var rm = 4000000;
                var trm = int.MaxValue;
                var tm = 20;
                var maxr = rm;
                var maxc = maxr;

                var rmi = 0;
                var tmi = 0;
                var trmi = int.MinValue;
                var minr = rmi;
                var minc = minr;
                
                var relevantRanges = new List<(int sc, int ec)>();

                foreach(var (sc, sr, bc, br, d) in sensorBeaconPairs)
                {
                        var distance = d;
                        //Console.WriteLine($"{sr} {sc} {br} {bc} {distance}");

                        var startR = sr - distance;
                        var endR = sr + distance;

                        if(startR <= tr && endR >= tr)
                        {
                                var rDist = Math.Abs(sr - tr);
                                var remDist = distance - rDist;
                                var startC = sc - remDist;
                                var endC = sc + remDist;
                                relevantRanges.Add((startC, endC));
                        }
                }

                Console.WriteLine($"RRs populated: {timer.ElapsedMilliseconds}");

                relevantRanges = relevantRanges.OrderBy(x => x.sc).ToList();
                var beaconPoints = sensorBeaconPairs.Where(x => x.br == tr).Select(x => (x.bc, x.br)).Distinct();

                var p1epc = 0;
                var p1cp = relevantRanges.First().sc;
                foreach(var rr in relevantRanges)
                {
                        var sp = Math.Max(rr.sc, p1cp);
                        var ep = rr.ec;
                        p1epc += Math.Max(ep - sp + 1, 0);

                        p1cp = Math.Max(p1cp, ep + 1);
                }

                var pointCount = p1epc - beaconPoints.Count();
                Console.WriteLine($"p1 calculated: {timer.ElapsedMilliseconds}");

                foreach(var (sc, sr, bc, br, d) in sensorBeaconPairs)
                {
                        var distance = d;
                        if(sr - distance > maxr || sr + distance < minr || sc - distance > maxc || sc+distance < minc)
                        {
                                // this sensor is not relevant for the target area
                                continue;
                        }

                        // iterate over the set of points just outside this beacon's range and check whether they are out of range
                        // of every other beacon
                        var startR = Math.Max(sr-distance, minr);
                        var endR = Math.Min(sr+distance, maxr);

                        var found = false;

                        for(int cr = Math.Max(startR - 1, minr); cr <= Math.Min(endR + 1, maxr); cr++)
                        {
                                var rDist = Math.Abs(sr - cr);
                                var remDist = (distance + 1) - rDist;

                                if(sc - remDist < minc || sc + remDist > maxc)
                                {
                                        continue;
                                }

                                var startC = Math.Max(sc - remDist, minc);
                                var endC = Math.Min(sc + remDist, maxc);

                                var oneCan = true;
                                var twoCan = true;
                                foreach(var (csc, csr, cbc, cbr, cd) in sensorBeaconPairs)
                                {
                                        var d1 = Math.Abs(cr - csr) + Math.Abs(startC - csc);
                                        var d2 = Math.Abs(cr - csr) + Math.Abs(endC - csc);
                                        if(d1 < cd)
                                        {
                                                oneCan = false;
                                        }
                                        if(d2 < cd)
                                        {
                                                twoCan = false;
                                        }
                                }
                                if(oneCan)
                                {
                                        found = true;
                                        p2 = new BigInteger(startC) * maxc + cr;
                                        Console.WriteLine($"Found It: {cr},{startC}");
                                        break;
                                }
                                if(twoCan)
                                {
                                        found = true;
                                        p2 = new BigInteger(endC) * maxc + cr;
                                        Console.WriteLine($"Found It: {cr},{endC}");
                                        break;
                                }
                        }

                        if (found)
                        {
                                Console.WriteLine($"p2 calculated: {timer.ElapsedMilliseconds}");
                                break;
                        }
                }

                Console.WriteLine( (pointCount.ToString(), p2.ToString()));
        }
}