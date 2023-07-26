// Camp Cleanup
namespace AOC22;
public class Day19 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var recipeDict = new Dictionary<int, Blueprint>();

                foreach(var line in lines)
                {
                        var ints = line.Split(' ').Select(x => x.TrimEnd(':')).Where(x => int.TryParse(x, out _)).Select(sbyte.Parse).ToArray();
                        var bp = new Blueprint
                        {
                                OreRecipe = ints[1],
                                ClayRecipe = ints[2],
                                ObsidianRecipe = (ints[3], ints[4]),
                                GeodeRecipe = (ints[5], ints[6]),
                                Index = ints[0]
                        };
                        recipeDict.Add(ints[0], bp);
                }

                var p1 = 0;

                var cachedCounts = new Dictionary<int, Dictionary<long, int>>();
                
                foreach(var (idx, bp) in recipeDict)
                {
                        GlobalMaxGeodes = 0;
                        var state = new State
                        {
                                OreRobots = 1,
                                TimeLeft = 24
                        };

                        var geodes = MaximizeGeodes(bp, state);

                        //Console.WriteLine($"{idx} {geodes}");

                        p1 += idx * geodes;

                        if(idx <= 3)
                        {
                                cachedCounts[idx] = GeodeCounts.ToDictionary(x => x.Key, x => x.Value);
                        }

                        //reset the cache to save memory
                        GeodeCounts.Clear();
                }


                var p2 = 1;
                foreach(var (idx, bp) in recipeDict.Where(x => x.Key <= 3))
                {
                        GlobalMaxGeodes = 0;
                        GeodeCounts = cachedCounts[idx];
                        var state = new State
                        {
                                OreRobots = 1,
                                TimeLeft = 32
                        };

                        var geodes = MaximizeGeodes(bp, state);

                        //Console.WriteLine($"{idx} {geodes}");

                        p2 *= geodes;
                }
                
                Console.WriteLine( (p1.ToString(), p2.ToString()));
        }       

        public Dictionary<long, int> GeodeCounts = new Dictionary<long, int>();

        public int GlobalMaxGeodes = 0;

        private int MaximizeGeodes(Blueprint recipe, State state)
        {
                var key = state.key;
                if(GeodeCounts.ContainsKey(key))
                {
                        return GeodeCounts[key];
                }

                if(state.TimeLeft == 1)
                {
                       return (int)state.GeodeRobots; 
                }


                // on every step we do the following
                // begin construction of new robots
                // collect ore from all existing robots
                // collect first but don't add to state
                // because construction will consume from state immediately
                // but collection doesn't happen until end of turn

                var oreToAdd = state.OreRobots;
                var clayToAdd = state.ClayRobots;
                var obsidianToAdd = state.ObsidianRobots;
                var geodeToAdd = state.GeodeRobots;


                var upperBound = GetUpperBoundForGeodes(recipe, state);

                // if there is no way to get higher than the current maximum even if we 
                // make a new geode bot every step of the way then just return
                if(upperBound < GlobalMaxGeodes)
                {
                        //Console.WriteLine("Short Circuit");
                        return 0;
                }

                var geodes = 0;
                var possibleToMake = 0;

                // We won't need more than this many ore bots to build any robot on any turn
                var maxOreBots = new[] {recipe.OreRecipe, recipe.ClayRecipe, recipe.GeodeRecipe.Item1, recipe.ObsidianRecipe.Item1}.Max();
                // The maximum amount of newly mined ore we could possibly use for the rest of the 
                var possibleOreDeficit = (maxOreBots * state.TimeLeft) - (state.OreRobots * state.TimeLeft) - state.Ore;

                // The maximum amount of newly mined clay we could possibly use for the rest of the 
                var possibleClayDeficit = (recipe.ObsidianRecipe.Item2 * state.TimeLeft) - (state.ClayRobots * state.TimeLeft) - state.Clay;

                var possibleObsidianDeficit = (recipe.GeodeRecipe.Item2 * state.TimeLeft) - (state.ObsidianRobots * state.TimeLeft) - state.Obsidian;

                if(state.Ore >= recipe.GeodeRecipe.Item1 && state.Obsidian >= recipe.GeodeRecipe.Item2)
                {
                        possibleToMake += 1;
                        var newState = State.CopyState(state);
                        newState.TimeLeft--;
                        newState.Ore -= recipe.GeodeRecipe.Item1;
                        newState.Obsidian -= recipe.GeodeRecipe.Item2;
                        newState.GeodeRobots += 1;
                        newState.Geodes += geodeToAdd;
                        newState.Ore += oreToAdd;
                        newState.Clay += clayToAdd;
                        newState.Obsidian += obsidianToAdd;
                        var newGeodes = MaximizeGeodes(recipe, newState);
                        geodes = Math.Max(geodes, newGeodes);
                }

                if(state.Ore >= recipe.ObsidianRecipe.Item1 && state.Clay >= recipe.ObsidianRecipe.Item2 && possibleObsidianDeficit > 0 && state.TimeLeft > 3)
                {
                        possibleToMake += 1;
                        var newState = State.CopyState(state);
                        newState.TimeLeft--;
                        newState.Ore -= recipe.ObsidianRecipe.Item1;
                        newState.Clay -= recipe.ObsidianRecipe.Item2;
                        newState.ObsidianRobots += 1;
                        newState.Geodes += geodeToAdd;
                        newState.Ore += oreToAdd;
                        newState.Clay += clayToAdd;
                        newState.Obsidian += obsidianToAdd;
                        var newGeodes = MaximizeGeodes(recipe, newState);
                        geodes = Math.Max(geodes, newGeodes);
                }

                if(state.Ore >= recipe.ClayRecipe && possibleClayDeficit > 0 && state.TimeLeft > 5)
                {
                        possibleToMake += 1;
                        var newState = State.CopyState(state);
                        newState.TimeLeft--;
                        newState.Ore -= recipe.ClayRecipe;
                        newState.ClayRobots += 1;
                        newState.Geodes += geodeToAdd;
                        newState.Ore += oreToAdd;
                        newState.Clay += clayToAdd;
                        newState.Obsidian += obsidianToAdd;
                        var newGeodes = MaximizeGeodes(recipe, newState);
                        geodes = Math.Max(geodes, newGeodes);
                }

                if(state.Ore >= recipe.OreRecipe && possibleOreDeficit > 0 && state.TimeLeft > 3)
                {
                        possibleToMake += 1;
                        var newState = State.CopyState(state);
                        newState.TimeLeft--;
                        newState.Ore -= recipe.OreRecipe;
                        newState.OreRobots += 1;
                        newState.Geodes += geodeToAdd;
                        newState.Ore += oreToAdd;
                        newState.Clay += clayToAdd;
                        newState.Obsidian += obsidianToAdd;
                        var newGeodes = MaximizeGeodes(recipe, newState);
                        geodes = Math.Max(geodes, newGeodes);
                }
                
                //if we can make everythnig then it's never correct to make nothing
                if (possibleToMake != 4)
                {
                        var newState = State.CopyState(state);
                        newState.TimeLeft--;
                        newState.Ore += oreToAdd;
                        newState.Clay += clayToAdd;
                        newState.Obsidian += obsidianToAdd;
                        newState.Geodes += geodeToAdd;
                        var newGeodes = MaximizeGeodes(recipe, newState);
                        geodes = Math.Max(geodes, newGeodes);
                }

                var retGeodes = geodes + geodeToAdd;
                GeodeCounts[key] = (int)retGeodes;
                GlobalMaxGeodes = Math.Max((int)retGeodes, GlobalMaxGeodes);
                return (int)retGeodes;
        }
        private int GetUpperBoundForGeodes(Blueprint recipe, State state)
        {
                var maxPossibleRemainingGeodes = state.Geodes + state.GeodeRobots * (state.TimeLeft + 1) + ((state.TimeLeft * (state.TimeLeft - 1)) / 2);
                var maxPossibleRemainingObsidian = state.Obsidian + state.ObsidianRobots * (state.TimeLeft + 1) + ((state.TimeLeft * (state.TimeLeft - 1)) / 2);

                // Imagine we got this much obsidian immediately
                // How many geode bots could we build and how many geodes would that give us?
                var maxGeodeBotsFromMaxObs = maxPossibleRemainingObsidian / recipe.GeodeRecipe.Item2;

                // imagine we could build all these bots starting in the next turn
                // this gives us a triangular number for how many geodes they can create
                var triangle = (maxGeodeBotsFromMaxObs * (maxGeodeBotsFromMaxObs - 1)) / 2;

                // once they're done they will all work for the remaining time
                var rectangle = (state.TimeLeft - maxGeodeBotsFromMaxObs) * maxGeodeBotsFromMaxObs;

                // Start by imagining we get all those geode bots immediately
                var maxGeodesFromObsLimit = state.Geodes + state.GeodeRobots * (state.TimeLeft + 1) + triangle + rectangle;

                var mathLimit = Math.Min(maxGeodesFromObsLimit, maxPossibleRemainingGeodes);

                // imagine a simplified simulatihn with some relaxations of the rules.
                // We'll say that you can actually create as many bots as you can afford in a single turn
                // we'll ignore the ore cost for obsidian and geode bots, and we'll allow double
                // use of ore to create clay and ore bots

                var clayOre = state.Ore;
                var oreOre = state.Ore;
                var clay = state.Clay;
                var obsidian = state.Obsidian;
                var geodes = state.Geodes;
                var clayBots = state.ClayRobots;
                var oreBots = state.OreRobots;
                var obsidianBots = state.ObsidianRobots;
                var geodeBots = state.ObsidianRobots;

                for(var i = 0; i < state.TimeLeft; i++)
                {
                        var oreToAdd = oreBots;
                        var clayToAdd = clayBots;
                        var obsidianToAdd = obsidianBots;
                        var geodesToAdd = geodeBots;

                        if(obsidian >= recipe.GeodeRecipe.Item2)
                        {
                                geodeBots++;
                                obsidian -= recipe.GeodeRecipe.Item2;
                        }

                        if(clay >= recipe.ObsidianRecipe.Item2)
                        {
                                obsidianBots++;
                                clay -= recipe.ObsidianRecipe.Item2;
                        }

                        if(clayOre >= recipe.ClayRecipe)
                        {
                                clayBots++;
                                clayOre -= recipe.ClayRecipe;
                        }

                        if(oreOre >= recipe.OreRecipe)
                        {
                                oreBots++;
                                oreOre -= recipe.OreRecipe;
                        }

                        oreOre += oreToAdd;
                        clayOre += oreToAdd;
                        clay += clayToAdd;
                        obsidian += obsidianToAdd;
                        geodes += geodesToAdd;
                }

                return Math.Min(mathLimit, geodes);
                //return mathLimit;
        }

        private record State
        {
                public static State CopyState(State state)
                {
                        var s = new State();
                        s.OreRobots = state.OreRobots;
                        s.ClayRobots = state.ClayRobots;
                        s.ObsidianRobots = state.ObsidianRobots;
                        s.GeodeRobots = state.GeodeRobots;
                        s.Ore = state.Ore;
                        s.Clay = state.Clay;
                        s.Obsidian = state.Obsidian;
                        s.Geodes = state.Geodes;
                        s.TimeLeft = state.TimeLeft;
                        return s;
                }

                public sbyte OreRobots; 
                public sbyte ClayRobots;
                public sbyte ObsidianRobots;
                public sbyte GeodeRobots;
                public sbyte Ore;
                public sbyte Clay;
                public sbyte Obsidian;
                public sbyte Geodes;
                public sbyte TimeLeft;

                public long key
                {
                        get
                        {
                                // This is the true magic
                                // The insight is that any state in which you have more than the maximum amount of an 
                                // ingredient for any of the recipes is identical
                                // ie if you need a maximum of 4 ore for any of the recipes then two
                                // otherwise identical states with 6 ond 10 ore are exctly the same
                                long retVal = 0;
                                retVal += ((long)OreRobots);
                                retVal += (((long)ClayRobots) << 8);
                                retVal += (((long)ObsidianRobots) << 16);
                                retVal += (((long)GeodeRobots) << 24);
                                retVal += (((long)Math.Min((byte)4, Ore)) << 32);
                                retVal += (((long)Math.Min((byte)20, Clay)) << 40);
                                retVal += (((long)Math.Min((byte)20, Obsidian)) << 48);
                                retVal += (((long)TimeLeft) << 56); // this won't be more than 5 bytes
                                return retVal;
                        }
                }
        }

        private class Blueprint
        {
                public sbyte Index;
                public sbyte OreRecipe {get; set;}
                public sbyte ClayRecipe {get; set;}
                public (sbyte, sbyte) ObsidianRecipe {get; set;}
                public (sbyte, sbyte) GeodeRecipe {get; set;}
        }
}