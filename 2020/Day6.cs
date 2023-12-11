using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace AOC20;
public class Day6 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                // This is needlessly golfed for my own amusement. It started out as many nested foreach loops
                // and ultimately became one giant aggregate over a list
                // 
                // There were several more reasonable steps along the way, like using a tuple rather than a list of lenght 2 but since tuples aren't enumerable
                // I couldn't get them into 1 line
                // I also had to use my own Enumerate function to index inte the accumulator list so I could do the list addition inline
                // to avoid that I could have used a Vector class that has native addition as the accumulator.
                //
                // The interesting addition while working on this was the new Split function in the Helpers class which lets us break up enumerables
                // into arbitrary sized chunks evere time we find whatever the "break" indicator is.
                // This should be handy for the AOC puzzles that break up chunks of data by newline where we end up building lists of lists and having to
                // add the sublist to the master list and remembering to do the last one outside the loop. This Split construction makes it very easy
                // to deal with this case.
                Console.WriteLine(string.Join(',', File.ReadAllLines(inputFile).Split(string.Empty).Aggregate(new List<int>() {0, 0}, (c, lg) => 
                        lg.Aggregate(new List<HashSet<char>>(){new(), lg.First().ToHashSet()}, (g1, l) => new List<HashSet<char>>(){g1[0].Union(l).ToHashSet(), g1[1].Intersect(l).ToHashSet()}).Select(x => x.Count)
                        .Enumerate().Select(x => c[x.i] + x.Item2).ToList()
                )));
        }

}