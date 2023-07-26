// There may be some small optimizations available here 
// like passing some "current min" into the recursion to
// allow computation to cut off if you end up over that
// still this is actually quite fast. The constraints on
// where the bugs will move and the small hallway constrains
// the state space pretty well.

// This is right at the edge of 1s, let's see if we can get it faster
// The first idea is to just use a long instead of a string for the cache key
// I don't remember the fisrt implementation but it looks like this array based 
// state system may have been built for performance as well
// no real need to switch to ints from chars, chars are small and fast and are
// really just ints under the covers anyway.

// Result: string is faster because it takes so much processing to get the
// nunber built up
//
// Tried a simple thing to track the minimum known cost vs the cost so far
// so we could stop recursing when the cost to make a move + the cost so far
// was higher than the minimum cost we've seen
// this only reduced the count by about 10% though.
//
// There are 2 things I can think of ;
// 1. Use some heuristic to try to order things by the "best" move
// That won't get a guaranteed best solution but may be a fast path to a
// decent solution which we can then use to constrain the searc.
// I don't really have a guess for that though. We could also find some
// algorithm to find a solution that might not be optimal but works and use 
// that to bound the search. That may be possible, it's some sort of weird
// towers of hanoi problem but I still don't know how to do it.
//
// 2. Modify the cutoff calculation with a minimun distance to completion
// calculation. That would iterate the state and find out if everything could move without being 
// blocked how mach would it cost to get everything in place. 
//
// That seems like a good idea, we'd probably have to estimate that every non correctly placed
// item only needs to get to the top of its home to make sure you're putting the right ones in the right places.
//
// Result: This reduces the amount of calls by about 13% but the calculation is too expensive
// also it got the answer wrong so it should be a bit worse than that. This could probably be improved by not
// calculating it every time but updating it and passing it. The minimun cost is whatever we first calculated
// minus the actual cost we have so far maybe?
//
// Another idea:
// When we're trying to get out of the home into the hallway we check all possible places we could go
// Once we're blocked though there's no need to check on the other side of that, if we're at 5 and we can't
// get to 7 we won't be able to get to 9 or 11 either.
//
// Result: This made a difference. Knocked off probably 18% of the execution time
//
// Another idea:
// We should add another state which is an occupancy mask. This would be cheap to maintain as a single inteeger
// and would allow for easy accessibility checks.
// 
// Result: This is actually sligthly worse because we still have to do the iteration to set up the 
// path mask each time. I think to fix this we'd need to have a set of pre calculated masks 
// to check occupancy between abitrary entries and exits
// I ended up skipping this because the found optimizations made up so much time
//
// Anothre idea:
// There are several spots in the hallway that we're not allowed to use (the ones right outside the rooms)
// but we still iterate them, we don't need to do that. 
//
// Result: Added the pointsToConsider array rather than iterating the entire state every time and it
// made a significant difference.
//
// Anothre idea:
// We iterate forward, within a home as soon as we find either a bug that is settled in the right spot
// or a bug that cannot get out we can skip forward to the next home
// 
// Result: this knocked off 100ms or so
//
// Another idea:
// We check if we're done on every iteration but on most iterations we're not
// Maybe we should pass a boolean in indicating whether it's possible that we're done
// That would only be the case when we've just put a bug into it's correct position
// from the hallway and that position is the last position in the home
// we can also optimize the done check to check homes layer by layer rather than
// an entire home at a time
//
// Result: First I did layer by layer, it maybe made a small difference but not much
// that makes sense because usually it probably fails on the first check. I'll try the
// bool thing too but thinking about it that probably won't make much difference for the same reason
// the done check is pretty cheap until very close to the end. Particularly if A is one of the later
// homes to be populated.
//
// Result: No real difference here. I should have measured the IsDone check 
//
// Found Optimization:
// When listing te set of desired points we were creating a list of the home points in a loop and then
// iterating them backwards to see if they were available or blocked. By making that a single iteration
// I cut off ~300ms
// That gets us comfortably under 1s
// Found optimization 2:
// The list of positions in the hall that we should try to go to was repopulated on each
// iteration but is actually static. Moving this out into a member variable made a noticeable difference.
// Found optimization 3

// Final Result:
// The found optimizations about not recreating the int arrays and not checkeng positions we know won't be 
// occupied made a big difference. I didn't neccesarily expect that so that's a good learning outcome.
// Any ideas of masking were tough because it took so many iterations to build the mask.
// same thing with the cache key. We'd need to build one at the start and then manipulate it over time
// since chars are just ints anyway they tend to be pretty fast.
// I think those optimizations could still make things faster but it would be a lot of work and we're already
// way under the 1s threshold I'm aiming for.

namespace AOC21;
public class Day23 : IDay
{
        private int[] hallPositions = new int[] { 0, 1, 3, 5, 7, 9, 10 };
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                // Input is in .in file for posterity but I'm just going to hardcode this
                // This feels like DP

                //  11 hallway positions + 8 home positions
                // format is first 11 are hallway left to right
                // next 2 are home 1, next home 2, etc
                var state = new char[19];
                for(var i = 0; i < 19; i++)
                {
                        state[i] = 'X';
                }
                state[11] = 'A';
                state[12] = 'D';
                state[13] = 'C';
                state[14] = 'D';
                state[15] = 'B';
                state[16] = 'A';
                state[17] = 'B';
                state[18] = 'C';
                var occupancyState = 0;
                
                foreach(var (i, c) in state.Enumerate())
                {
                        if(c == 'X')
                        {
                                continue;
                        }
                        else
                        {
                                occupancyState += (1 << i);
                        }
                }

                var sampleState = new char[19];
                for(var i = 0; i < 19; i++)
                {
                        sampleState[i] = 'X';
                }
                sampleState[11] = 'B';
                sampleState[12] = 'A';
                sampleState[13] = 'C';
                sampleState[14] = 'D';
                sampleState[15] = 'B';
                sampleState[16] = 'C';
                sampleState[17] = 'D';
                sampleState[18] = 'A';

                // This should be slightly faster than enumerating the state array because
                // we can skip the hall points that are never occupeied
                pointsToConsider = hallPositions.Concat(Enumerable.Range(11,8)).ToArray();

                Console.WriteLine(CostToGround(state, 2, 0, long.MaxValue, false));

                var p2State = new char[27];
                for(var i = 0; i < 27; i++)
                {
                        p2State[i] = 'X';
                }
                p2State[11] = 'A';
                p2State[12] = 'D';
                p2State[13] = 'D';
                p2State[14] = 'D';

                p2State[15] = 'C';
                p2State[16] = 'C';
                p2State[17] = 'B';
                p2State[18] = 'D';

                p2State[19] = 'B';
                p2State[20] = 'B';
                p2State[21] = 'A';
                p2State[22] = 'A';

                p2State[23] = 'B';
                p2State[24] = 'A';
                p2State[25] = 'C';
                p2State[26] = 'C';

                var p2OccupancyState = 0;
                
                foreach(var (i, c) in p2State.Enumerate())
                {
                        if(c == 'X')
                        {
                                continue;
                        }
                        else
                        {
                                p2OccupancyState += (1 << i);
                        }
                }

                var p2sampleState = new char[27];
                for(var i = 0; i < 27; i++)
                {
                        p2sampleState[i] = 'X';
                }
                p2sampleState[11] = 'B';
                p2sampleState[12] = 'D';
                p2sampleState[13] = 'D';
                p2sampleState[14] = 'A';

                p2sampleState[15] = 'C';
                p2sampleState[16] = 'C';
                p2sampleState[17] = 'B';
                p2sampleState[18] = 'D';

                p2sampleState[19] = 'B';
                p2sampleState[20] = 'B';
                p2sampleState[21] = 'A';
                p2sampleState[22] = 'C';

                p2sampleState[23] = 'D';
                p2sampleState[24] = 'A';
                p2sampleState[25] = 'C';
                p2sampleState[26] = 'A';

                // This should be slightly faster than enumerating the state array because
                // we can skip the hall points that are never occupeied
                pointsToConsider = hallPositions.Concat(Enumerable.Range(11,16)).ToArray();
                Console.WriteLine(CostToGround(p2State, 4, 0, long.MaxValue, false));

                Console.WriteLine(counter);

        }

        Dictionary<string, long> cache = new Dictionary<string, long>();

        long counter = 0;

        private int[] pointsToConsider;

        public long CostToGround(char[] state, int homeSize, long costSoFar, long minSoFar, bool maybeDone)
        {
                if(maybeDone && IsDone(state, homeSize))
                {
                        return 0;
                } 

                var ck = new string(state);

                if(cache.ContainsKey(ck))
                {
                        //Console.WriteLine("Cache Hit!");
                        return cache[ck];
                }

                var minCost = long.MaxValue;

                // if the minimum it could possibly cost us to ground is larger than the
                // smallest case we've seen so far then bail
                //var minPossibleCost = MinPossibleCost(state, homeSize);
                //if(minPossibleCost > minSoFar)
                //{
                        //return minCost;
                //}
                counter++;

                for(var ii = 0; ii < pointsToConsider.Length; ii++)
                {
                        var i = pointsToConsider[ii];
                        var c = state[i];
                        if(c == 'X')
                        {
                                continue;
                        }

                        var (cost, homeEntry, dp) = GetSpaceInfo(state, i, homeSize);

                        if (dp == 0)
                        {
                                // if we're in home and we're already settled then we can skip ahead to the next home
                                if(i > 10)
                                {
                                        var homeDepth = (i - 11) % homeSize;
                                        var skipDistance = homeSize - homeDepth - 1;
                                        ii += skipDistance;
                                }
                                // We found ourseleves in the list this bug is already correctly placed
                                continue;
                        }

                        // We're in the hallway
                        if(i <= 10)
                        {
                                // Our destination is blocked, this bug can't move
                                if(dp == -1)
                                {
                                        continue;
                                }
                                // we found a bug in the hallway
                                // we need to see if we can send it home
                                // we can send it home iff the bottom spot is unoccupied or occupied by the right type
                                // and the path is unblocked

                                var viable = true;
                                // check if the path to the home entry is unbloced
                                if (i > homeEntry)
                                {
                                        for (var j = i - 1; j >= homeEntry; j--)
                                        {
                                                if (state[j] != 'X')
                                                {
                                                        viable = false;
                                                        break;
                                                }
                                        }
                                }
                                else
                                {
                                        for (var j = i + 1; j <= homeEntry; j++)
                                        {
                                                if (state[j] != 'X')
                                                {
                                                        viable = false;
                                                        break;
                                                }
                                        }
                                }

                                if(!viable)
                                {
                                        continue;
                                }
                                else
                                {
                                        var homeDepth = (dp - 11) % homeSize;
                                        var dist = Math.Abs(homeEntry - i) + homeDepth + 1; // distance down the halway + how far down the home to go

                                        var newState = new char[11 + (4 * homeSize)];
                                        state.CopyTo(newState, 0);
                                        newState[i] = 'X';
                                        newState[dp] = c;

                                        //Console.WriteLine($"Found bug {c} in hallway {i} with open target {dp} and path");

                                        var myCost = cost * dist;
                                        // If there's no way we'll come in under the lowest cost so far
                                        // we can bail now
                                        if(costSoFar + myCost >= minCost)
                                        {
                                                continue;
                                        }

                                        var maybeFinishedThisStep = homeDepth == 0;
                                        var nextCost = CostToGround(newState, homeSize, costSoFar + myCost, minCost, maybeFinishedThisStep);
                                        if(nextCost == long.MaxValue)
                                        {
                                                continue;
                                        }

                                        var newCost = myCost + nextCost;
                                        //Console.WriteLine($"New Cost: {newCost}");
                                        minCost = Math.Min(minCost,  newCost);
                                }
                        }
                        else
                        // Nothing viable to do in the hallway, let's put one of the home ones in the hallway
                        {

                                // Check if we can get out of here
                                var canGetOut = true;
                                var homeDepth = (i - 11) % homeSize;
                                for(var j = i - 1; j >= i - homeDepth; j--)
                                {
                                        if(state[j] != 'X')
                                        {
                                                canGetOut = false;
                                                break;
                                        }
                                }
                                // if we can't get out then nobody below us can either so skip ahead
                                if(!canGetOut)
                                {
                                        var skipDistance = homeSize - homeDepth - 1;
                                        ii += skipDistance;
                                        continue;
                                }

                                var homeExit = (((i - 11) / homeSize) * 2) + 2;

                                var leftFound = 0;
                                var rightDone = false;

                                // Now check whether we can actually get to any of these hallway points
                                foreach(var target in hallPositions)
                                {
                                        var viable = true;
                                        // check if the path to the home entry is unblocked
                                        if (target > homeExit)
                                        {
                                                // the target is to the right of us
                                                for (var j = homeExit + 1; j <= target; j++)
                                                {
                                                        if (state[j] != 'X')
                                                        {
                                                                viable = false;
                                                                rightDone = true;
                                                                break;
                                                        }
                                                }
                                        }
                                        else
                                        {
                                                if(leftFound > target)
                                                {
                                                        continue;
                                                }
                                                for (var j = homeExit - 1; j >= target; j--)
                                                {
                                                        if (state[j] != 'X')
                                                        {
                                                                viable = false;
                                                                leftFound = j;
                                                                break;
                                                        }
                                                }
                                        }
                                        if(rightDone)
                                        {
                                                break;
                                        }
                                        if(!viable)
                                        {
                                                continue;
                                        }
                                        else
                                        {
                                                var newState = new char[11 + (4 * homeSize)];
                                                state.CopyTo(newState, 0);
                                                newState[i] = 'X';
                                                newState[target] = c;

                                                var dist = Math.Abs(target - homeExit) + 1 + homeDepth;
                                                //Console.WriteLine($"Found bug {c} in home {homeExit} with viable target {target}");

                                                var myCost = cost * dist;
                                                // If there's no way we'll come in under the lowest cost so far
                                                // we can skip recursing down through it
                                                if (costSoFar + myCost >= minCost)
                                                {
                                                        continue;
                                                }
                                                var nextCost = CostToGround(newState, homeSize, myCost + costSoFar, minCost, false);
                                                if (nextCost == long.MaxValue)
                                                {
                                                        continue;
                                                }

                                                var newCost = (cost * dist) + nextCost;
                                                //Console.WriteLine($"New Cost: {newCost}");
                                                minCost = Math.Min(minCost, newCost);
                                        }
                                }

                        }
                }

                // No Valid Moves
                cache[ck] = minCost;
                return minCost;
        }

        public (int cost, int homeEntry, int dp) GetSpaceInfo(char[] state, int i, int homeSize)
        {
                var desiredPoints = new List<int>();
                var homeEntry = -1;

                var dpStart = 11;

                var cost = 0;
                var c = state[i];
                switch (c)
                {
                        case 'A':
                                dpStart = 11;
                                cost = 1;
                                homeEntry = 2;
                                break;
                        case 'B':
                                dpStart = 11 + homeSize;
                                cost = 10;
                                homeEntry = 4;
                                break;
                        case 'C':
                                dpStart = 11 + (2 * homeSize);
                                cost = 100;
                                homeEntry = 6;
                                break;
                        case 'D':
                                dpStart = 11 + (3 * homeSize);
                                cost = 1000;
                                homeEntry = 8;
                                break;
                }

                var dp = 0;
                // iterate up from the bottom of our desired home
                // If we see letters that belong, continue
                // If we see ourselves we're done and this char
                // shouldn't be considered going forward
                // If we see an 'X' that's where we want to be
                // If we see another letter we can't move into 
                // home so the hall is our only option
                for(var idx = dpStart + homeSize - 1; idx >= dpStart; idx--)
                {
                        // We're in the right spot
                        if (state[idx] == c && i == idx)
                        {
                                break;
                        }
                        if (state[idx] == c)
                        {
                                continue;
                        }
                        if (state[idx] == 'X')
                        {
                                dp = idx;
                                break;
                        }
                        else
                        {
                                dp = -1;
                                break;
                        }
                }
                return (cost, homeEntry, dp);

        }

        // These are the positions to check layer by layer
        private int[] homeIter2 = new[] { 11, 13, 15, 17, 12, 14, 16, 18};
        private int[] homeIter4 = new[] { 11, 15, 19, 23,  12, 16, 20, 24,  13, 17, 21, 25,  14, 18, 22, 26};

        public bool IsDone(char[] state, int homeSize)
        {
                var count = homeSize * 4;
                var iter = homeSize == 2 ? homeIter2 : homeIter4;
                foreach(var i in iter)
                {
                        var home = (i - 11) / homeSize;
                        switch(home)
                        {
                                case 0:
                                        if(state[i] != 'A')
                                        {
                                                return false;
                                        }
                                        break;
                                case 1:
                                        if(state[i] != 'B')
                                        {
                                                return false;
                                        }
                                        break;
                                case 2:
                                        if(state[i] != 'C')
                                        {
                                                return false;
                                        }
                                        break;
                                case 3:
                                        if(state[i] != 'D')
                                        {
                                                return false;
                                        }
                                        break;
                        }
                }
                return true;
        }

}