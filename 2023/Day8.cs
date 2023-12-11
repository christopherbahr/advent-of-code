/*
This was a pretty easy problem disguised as a very difficult problem. Definitely a good lesson in how AoC works.

The part 2 implementation has you walking several different paths simultaneously and looking for the time when
they all are at a valid end state at the same time. Given the problem it's obvious that there will be loops
but solving that generally is extremely difficult.

Rather than dissceting the input a bit I went straight for the general solution. It was only when printing 
out some intermediate states that I noticed several interesting things. They felt so unexpected I half thought they were bugs.

Attempts:
1. I first tried the extremely naive approach of just simulating all of them. I wasn't surprised when that didn't work but you never know.
2. Then I tried a DP approach. The idea was to find and store the distance from any node to the next z. That caused a stack overflow because
   the paths were too long. It was a bad idea anyway though because (as I realized shortly after) the only nodes that we really care about 
   are the start nodes and the end nodes.
3. The above realization is that once you're in the looping phase you really only care about how long it takes to get from one end node
   to the next end node. That (theoretically) would vary based on which point in the directions you were in when you got to the end node.
   The distance from each starting node to its first ending node can just be computed using the part 1 code. Then I generated a cache of 
   the distance from one Z node to the next for each starting instruction. There are only 6 end nodes and only 283 instructions so this
   worked out to be reasonable. I stored the distance between end nodes and how many instructions it took to get between them. Then I wrote
   a loop that would simulate each ghost individually, stepping whichever one had gone the fewest steps each time. It would step them forward
   to their next end node and step their individual counter and instruction pointer forward the right amount. I believe this is a fully 
   general solution but it was much too slow. 
   It felt like a chinese remainder theorem problem but I was struggling to reason about all the possible variations. Inspecting the various 
   steps of this solution I made the critical discovery that the only loops I was ever using were ones that looped back to themselves and 
   ended up on the 0 instruction pointer. That made things hugely simpler.  4. With the realization that the loops were looping back on 
   themselves and always at the same instruction pointer I was able to come up with a system of equations for the chinese remainder theorem. 
   Simply the end_times = distance_from_start_to_first_end % length_of_end_loop.  I plugged this into my handy dandy CRT calculator which 
   blew up because the loops weren't coprime (incidentally I sholud have guessed this because they were obviously all divisible by the l
   ength of the direction list).
5. Before I could figure out what to do about the CRT (which I only loosely understand) I realized that the distance from the start to the 
   first endpoint was the same as the loop distance in all cases. That means we just need to find the least common multiple of those numbers
   and we'll know when the ghosts are all at and endpoint at the same time.

Lesson Learned: Poke at your input before trying to write a general solution. See if there are any interesting patterns
and then try to exploit them.
*/

namespace AOC23;

public class Day8 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        readonly Dictionary<string, (string self, string l, string r)> nodes = new Dictionary<string, (string self, string l, string r)>();
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var directions = lines.First();
                foreach(var line in lines.Skip(2))
                {
                        var stuff = line.Split('=');
                        var source = stuff[0].Trim();
                        var lr = stuff[1].Split(',');
                        var l = lr[0].Trim().Trim('(').Trim();
                        var r = lr[1].Trim(')').Trim();
                        nodes[source] = (source, l, r);
                }

                //PART 1 IMPLEMENTATION
                var p1 = DistanceToZ("AAA", directions);

                // Because all the distances from a nodes to Z nodes are the same as the lengths of the loops (see long comment above)
                // we really only need to find one number here.
                var p2 = LCM(nodes.Where(x => x.Key.EndsWith('A')).Select(x => DistanceToZ(x.Key, directions)));
                Console.WriteLine((p1, p2));
        }

        public long DistanceToZ(string node, IEnumerable<char> directions)
        {
                var counter = 0;
                var curNode = nodes[node];
                while(true)
                {
                        foreach(var (i, d) in directions.Enumerate())
                        {
                                counter++;
                                curNode = d == 'L' ? nodes[curNode.l] : nodes[curNode.r];
                                if(curNode.self.EndsWith('Z'))
                                {
                                        return counter;
                                }
                        }
                }
        }

        static long LCM(IEnumerable<long> numbers)
        {
                return numbers.Aggregate(lcm);
        }

        static long lcm(long a, long b)
        {
                return Math.Abs(a * b) / GCD(a, b);
        }

        static long GCD(long a, long b)
        {
                return b == 0 ? a : GCD(b, a % b);
        }

}