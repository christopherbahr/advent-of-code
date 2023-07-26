// Supply Stacks
namespace AOC22;
public class Day5 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                // take until there is an empty line
                var setupLines = lines.TakeWhile(x => x != string.Empty);

                var stacks = ParseSetup(setupLines);
                var fancyCraneStacks = ParseSetup(setupLines);

                // Skip until you find and empty line, then start going
                var instructionsLines = lines.SkipWhile(x => x != string.Empty).Skip(1);

                foreach (var instruction in instructionsLines)
                {
                        var components = instruction.Split(' ');

                        var count = int.Parse(components[1]);
                        // -1 to account for the fact that elves used 1 based indexing
                        var source = int.Parse(components[3]) - 1;
                        var dest = int.Parse(components[5]) - 1;

                        // move one item at a time
                        for(var i = 0; i < count; i++)
                        {
                                stacks[dest].Push(stacks[source].Pop());
                        }

                        // move many items
                        // note a stack is a dumb data structure for this new problem but I don't want to rewrite the parser to be generat
                        // instead we'll just use a temporary stack, pop n items off the old stack into the temp stack, then pop them back 
                        // off the temp stack into the target stack. That's dumb but it will work

                        var tempStack = new Stack<char>();
                        for(var i = 0; i < count; i++)
                        {
                                tempStack.Push(fancyCraneStacks[source].Pop());
                        }
                        for(var i = 0; i < count; i++)
                        {
                                fancyCraneStacks[dest].Push(tempStack.Pop());
                        }
                }

                var topCrates = new string(stacks.Select(x => x.Peek()).ToArray());
                var fancyTopCrates = new string(fancyCraneStacks.Select(x => x.Peek()).ToArray());

                Console.WriteLine( (topCrates, fancyTopCrates));
        }

        public IList<Stack<char>> ParseSetup(IEnumerable<string> setup)
        {
                // First reverse it so we can load the stacks from the bottom.
                var stackedSetup = setup.Reverse();

                // how many stacks are there
                var lastChars = setup.Last().Split(' ').Select(x => x.Trim());

                var count = lastChars.Where(x => x!= string.Empty).Select(int.Parse).Last();

                var stacks = new List<Stack<char>>(count);

                for (var i = 0; i < count; i++)
                {
                        stacks.Add(new Stack<char>());
                }

                // It would be most elegant to record the index of the number so we would tolerate
                // arbitrary amounts of spaces between the stacks
                // For now we won't bother though, we'll just write the formula for which index
                // a letter belongs in depending on which index it's in in the string

                // 1 => 0, 5 => 1, 9 => 2 So we have strIndex = 4 * stackIndex + 1
                //


                foreach(var line in stackedSetup.Skip(1))
                {
                        for(int i = 0; i < line.Length; i++)
                        {
                                var c = line[i];
                                if(c >= 'A' && c <= 'Z') // we have a character
                                {
                                        var stackIdx = i / 4;
                                        stacks[stackIdx].Push(c);
                                }
                        }
                }

                return stacks;
        }

}