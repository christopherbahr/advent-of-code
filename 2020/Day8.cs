namespace AOC20;
public class Day8 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var (_, acc) = Simulate(lines);
                Console.WriteLine(acc);

                // It really seems like there should be a better way to do this
                // The idea is that we have to swap one nop -> jmp or one jmp -> nop 
                // such that the program terminates
                //
                // It seems like we should be able to find the number of instructions at the end
                // that would fall nicely through to termination and then find any jump that lands there
                // or a nop that would jump there if it switched.
                //
                // If a jump lands there iterate the same process but with that jump (rather than the instruction beyond the end)
                // as the target. If a nop that would land there if switched is found switch it and you're done.
                //
                // Practically though with only a few hundred relevant instructions we can just
                // simulate the whole thing repeatedly
                var lc = new string[lines.Length];
                lines.CopyTo(lc, 0);
                foreach (var (i, line) in lines.Enumerate())
                {
                        var instData = line.Split(' ');
                        var cmd = instData[0];
                        if(cmd == "acc")
                        {
                                continue;
                        }
                        var oldCmd = lc[i];
                        lc[i] = $"{(cmd == "nop" ? "jmp" : "nop")} {instData[1]}";
                        var (term, acc2) = Simulate(lc);
                        if(term)
                        {
                                Console.WriteLine(acc2);
                                break;
                        }
                        lc[i] = oldCmd;
                }
        }

        public (bool terminated, int acc) Simulate(string[] lines)
        {
                var instPtr = 0;
                var acc = 0;
                var seen = new  HashSet<int>();
                while(true)
                {
                        if(seen.Contains(instPtr))
                        {
                                break;
                        }
                        seen.Add(instPtr);
                        if(instPtr == lines.Length)
                        {
                                return (true, acc);
                        }
                        var inst = lines[instPtr];
                        var instData = inst.Split(' ');
                        var cmd = instData[0];
                        var amt = int.Parse(instData[1].Trim('+'));
                        switch(cmd)
                        {
                                case "acc":
                                        acc += amt;
                                        instPtr++;
                                        break;
                                case "jmp":
                                        instPtr += amt;
                                        break;
                                case "nop":
                                        instPtr++;
                                        break;
                        }
                }
                return (false, acc);
        }

}