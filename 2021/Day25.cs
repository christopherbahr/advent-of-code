// Notes:
// Originally this just naively manipulated the grid of characters
// it would process every character twice per iteration first checking if
// it was east, then trying to move it, then a second pass checking for south
// movement.
// Then changing the contents of the 2d char array by changing only the positions that moved
// That was just a bit slower than 1s to solve the problem.

// The first idea was to populate hash sets of east ponits and south points
// then iterate those sets so you only ever try to process occupied points and
// you only try to process them once.
// that was faster than 1s but only barely.
// The cost is probably from deleting and repopulating the hash sets so many times
// plus the cost of checking occupancy is higher with a hashset than by directly indexing it

// Wait.. why are we repopulating this entirely? Can't we just edit the existing one?
// Oh because we can't edit the hash set while we're iterating. 
// So maybe back to collecting moves and then making them?
// Result: That's much faster

// I think the right answer is to keep a list of bit arrays one for east and one for south
// each entry in the list is a row or column
// Oh actually  i only figured this out for going east.. 
// Anyway imagine that each entry is a row
// Then the set of occupied places in a row is occupied = eastHerd[i] & southHerd[i]
// the set of desired places is shifted = eastHerd[i] >> 1 (plus some magic to deal with the wrap around behavior)
// The set of uwnblocked desired places is moved = (shifted ^ occupied) & shifted
// The set of unmoved cucumbers is shifted & occupied
// I think the way to actually do this is to store the whole thing as one long array
// Moving east is moving 1 space, moving south is moving arrLen spaces
// Then you can do nice masking and jumping
// At the boundary if you're trying to move south into a spot that's off the edge just go 
// to (i % arrLen)
// Whenever you're going east you want to go to ((i % rowLen)* rowLen) + (i + 1) % rowLen
// The first term gets your row, the second gets your position in the row
// 
// Still getting the masking right is tough because your state isn't just occupation but 
// also direction.
//
// Anyway the above optimization got us well under the 1s thresold so I'll call it good.

namespace AOC21;
public class Day25 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var eastHerd = new HashSet<(int r, int c)>();
                var southHerd = new HashSet<(int r, int c)>();

                var g = lines.AsCharGrid();
                foreach(var (p, ch) in lines.AsCharGrid().Enumerate())
                {
                        if(ch == '>')
                        {
                                eastHerd.Add(p);
                        }
                        else if(ch == 'v')
                        {
                                southHerd.Add(p);
                        }
                }
                var colCount = g.GetLength(1);
                var rowCount = g.GetLength(0);
                var movedEast = true;
                var movedSouth = true;
                var counter = 0;
                var toAdd = new List<(int, int)>(eastHerd.Count + southHerd.Count);
                while(movedEast || movedSouth)
                {
                        movedEast = false;
                        movedSouth = false;
                        // first east
                        foreach(var p in eastHerd)
                        {
                                var (cr, cc) = (p.r + Helpers.East.Item1, p.c + Helpers.East.Item2);
                                if(cc >= colCount)
                                {
                                        cc = 0;
                                }
                                var cp = (cr, cc);
                                if (!eastHerd.Contains(cp) && !southHerd.Contains(cp))
                                {
                                        movedEast = true;
                                        toAdd.Add(cp);
                                }
                        }
                        foreach(var (r, c) in toAdd)
                        {
                                eastHerd.Add((r, c));
                                var remC = c - 1;
                                if(c == 0)
                                {
                                        remC = colCount - 1;
                                }
                                eastHerd.Remove((r, remC));
                        }
                        toAdd.Clear();
                        foreach(var p in southHerd)
                        {
                                var (cr, cc) = (p.r + Helpers.South.Item1, p.c + Helpers.South.Item2);
                                if (cr >= rowCount)
                                {
                                        cr = 0;
                                }
                                var cp = (cr, cc);
                                if (!eastHerd.Contains(cp) && !southHerd.Contains(cp))
                                {
                                        movedSouth = true;
                                        toAdd.Add(cp);
                                }
                        }
                        foreach(var (r, c) in toAdd)
                        {
                                southHerd.Add((r, c));
                                var remR = r - 1;
                                if(r == 0)
                                {
                                        remR = rowCount - 1;
                                }
                                southHerd.Remove((remR, c));
                        }
                        toAdd.Clear();
                        counter++;
                }
                Console.WriteLine(counter);

        }

}