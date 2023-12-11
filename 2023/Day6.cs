namespace AOC23;
public class Day6 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                // I've left the very simple algorithm for the first part 
                // This could also be changed to a quadratic but to show
                // the simple one that works for p2 but is a bit slow I'm leaving it
                // there's a small chance this is actually faster anyway for small
                // inputs because of the expensive math that goes into the quadratic
                // approach
                var lines = File.ReadAllLines(inputFile);
                var times = lines.First().GetInts();
                var distances = lines.Skip(1).First().GetInts();
                var tds = times.Zip(distances);
                var p1 = 1L;
                foreach(var (t, d) in tds)
                {
                        var counter = 0;
                        foreach(var ht in Enumerable.Range(0, t))
                        {
                                var rt = t - ht;
                                var rd = ht * rt;
                                if(rd > d)
                                {
                                        counter++;
                                }
                        }
                        p1 *= counter;
                }
                Console.WriteLine(p1);

                var raceTime = long.Parse(string.Join("", lines.First().GetInts()));
                var record = long.Parse(string.Join("",lines.Skip(1).First().GetInts()));

                // even using this extremely naive algorithm we finish in a bit over 100 ms
                // an obvious first step would be to iterate until you fand one, then iterate backwards
                // until you find one, and then say that everything in between is a winner.
                // Even better than that would be to binary search for the first and last passing points
                // Anyway we don't really need to do any of that because this is really just a math problem.
                //
                // racedDistance = (raceTime - holdTime) * raceSpeed
                // raceSpeed = holdTime so we simplify to
                // racedDistance = raceTime * holdTime - holdTime^2
                // -holdTime^2 + raceTime * holdTime - racedDistance = 0
                // This makes an inverse parabola. If we set racedDistance to the record + 1
                // then the x intercepts will be the first and last hold times that would 
                // work.
                // This is just the quadratic equation
                // holdTime = -(-raceTime +/- sqrt(raceTime * raceTime - 4(record + 1))) / 2
                // holdTime = (raceTime +/- sqrt(raceTime * raceTime - 4(record + 1))) / 2
                
                var rootTerm = Math.Sqrt(raceTime * raceTime - (4 * (record + 1)));
                var numerator1 = raceTime + rootTerm;
                var numerator2 = raceTime - rootTerm;

                var firstHt = numerator1 / 2;
                var secondHt = numerator2 / 2;

                // we want the first whole number of ms that would work
                // Floor will be the last one that doesn't beat the record
                // The same explanation in reverse for why we choose Floow 
                // for the sonced one.
                // Then we add 1 to deal with the boundary (ie if the winners are from 1-4 we want to come up with 4 not 3)
                var firstIntHt = Math.Ceiling(firstHt);
                var secondIntHt = Math.Floor(secondHt);
                var p2 = Math.Abs(secondIntHt - firstIntHt) + 1;

                Console.WriteLine(p2);
        }
}
