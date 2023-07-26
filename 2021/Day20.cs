// This is one that takes more than a second
// First thought is that we can track the min & max row and column
// more efficiently than recalculating it each time
// Result: Maybe sligthly faster? Hard to say. Nothing big

// Second idea: algorithm is a string and we're doing char compares. Switch to bool array
// Result: Also maybe a tiny improvement. Nothing dramatic

// Third idea: We're calculating the lookup for each 3x3 box independently
// but consecutive boxes actually have 2/3 of their numbers overplap.
// if we're just moving 1 to the right we can do algoPos &= 011011011 and clear out the
// 3 that are no longer relevant. Then algoPos = algoPos << 1 to get the bits into the right 
// spot. Then we can just check NE, E, SE and put them in the right spots

// Result: This got me down under a second but it's still not as much faster as I expected
// I think this should have cut off about 2/3 of the work by clearing out those iterations
// 
// A small improvement was also to allocate the new hash sets with enough space for 2x more data
// than was in the previous picture.
// 
// Still we're under a second so I guess that's fine
//
// Another idea would be to parallelize this since each node is independent it could be on different threads.
//
// Another idea:
// Don't recalculate dirs & enumDirs over and over. Set them up once and share them between iterations.
// This is similar to an important optimization for day 23
// Result: cut the time nearly in half!


using static Helpers;

namespace AOC21;
public class Day20 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        (int, (int, int))[] enumDirs = new[] { (6, NorthEast), (3, East), (0, SouthEast) }.Reverse().ToArray();
        (int, (int, int))[] dirs;
        public void Execute()
        {
                var fullDirs = new[] { NorthWest, North, NorthEast, West, Center, East, SouthWest, South, SouthEast }.Reverse().ToArray();
                dirs = fullDirs.Enumerate().ToArray();
                var lines = File.ReadAllLines(inputFile);

                var algoStr = lines.First();
                var algo = algoStr.Select(x => x == '#').ToArray();
                var pic = new HashSet<(int, int)>();
                foreach(var ((r, c), ch) in lines.Skip(2).CharGrid())
                {
                        if(ch == '#')
                        {
                                pic.Add((r, c));
                        }
                }
                // because of our algo the non-lit squares on the edge fully light each turn
                // then the fully lit squares unlight each turn. 
                // we have to account for that
                var minr = pic.Min(x => x.Item1);
                var maxr = pic.Max(x => x.Item1);
                var minc = pic.Min(x => x.Item2);
                var maxc = pic.Max(x => x.Item2);
                var dropMinR = false;
                var dropMinC = false;
                var incMaxR = false;
                var incMaxC = false;
                // we want this to be 011_011_011
                // that way we clear out the western bits
                // then we shift the whole thing west one bit 
                // so that the eastern bits are clear
                // and the middlle bits become west
                // and the eastern bits become middle
                var algoResetMask = 1;
                algoResetMask += (1 << 1);
                algoResetMask += (1 << 3);
                algoResetMask += (1 << 4);
                algoResetMask += (1 << 6);
                algoResetMask += (1 << 7);

                for(var k = 0; k < 50; k++)
                {
                        dropMinR = false;
                        dropMinC = false;
                        incMaxR = false;
                        incMaxC = false;
                        var newPic = new HashSet<(int, int)>(pic.Count * 2);
                        for(var cr = minr - 1; cr <= maxr + 1; cr++)
                        {
                                int algoPos = 0;
                                for(var cc = minc - 1; cc <= maxc+1; cc++)
                                {
                                        // Clears out the western bits of the lookup pointer
                                        // if we've just reset a row this will have no effect
                                        // because we will have just reset the algorithm

                                        // we will only repopulate the eastern bits in that case
                                        // reset western bits
                                        algoPos &= algoResetMask;
                                        // shift middle -> west, east -> middle since we're shifted the window
                                        // east one cell
                                        algoPos = algoPos << 1;
                                        // we've just reset, we need to fully repopulate the lookup pointer
                                        var myEnumDirs = enumDirs;
                                        if(cc == minc -1)
                                        {
                                                myEnumDirs = dirs;
                                        }
                                        foreach( var (i, (dr, dc)) in (cc == minc - 1 ? dirs : enumDirs))
                                        {
                                                var pos = (cr +dr, cc + dc);
                                                var set = false;
                                                // account for the boundary
                                                // if we are at the boandary and processing an 
                                                // odd number step the whole boundary should be lit up
                                                if(pos.Item1 < minr || pos.Item1 > maxr || pos.Item2 < minc || pos.Item2 > maxc)
                                                {
                                                        if(k % 2 != 0)
                                                        {
                                                                set = true;
                                                        }
                                                }
                                                if(set || pic.Contains((cr + dr, cc + dc)))
                                                {
                                                        algoPos += (1 << i);
                                                }
                                        }
                                        var isLit = algo[algoPos];
                                        if(isLit)
                                        {
                                                if(cr < minr)
                                                {
                                                        dropMinR = true;;
                                                }
                                                if(cr > maxr)
                                                {
                                                        incMaxR = true;
                                                }
                                                if(cc < minc)
                                                {
                                                        dropMinC = true;
                                                }
                                                if(cc > maxc)
                                                {
                                                        incMaxC = true;
                                                }
                                                newPic.Add((cr, cc));
                                        }
                                }
                        }
                        if(dropMinR)
                        {
                                minr--;
                        }
                        if(incMaxR)
                        {
                                maxr++;
                        }
                        if(dropMinC)
                        {
                                minc--;
                        }
                        if(incMaxC)
                        {
                                maxc++;
                        }
                        pic = newPic;
                        if(k == 1)
                        {
                                Console.WriteLine(pic.Count());
                        }
                }
                Console.WriteLine(pic.Count());
        }
}