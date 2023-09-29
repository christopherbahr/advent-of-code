using System.Numerics;

namespace AOC22;
public class Day21 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var treeDict = new Dictionary<string, Operation>();

                foreach(var line in lines)
                {
                        var parts = line.Split(' ');
                        if(parts.Count() == 4)
                        {
                                treeDict[parts[0].TrimEnd(':')] = new Operation{Arg1 = parts[1], Operator = parts[2], Arg2 = parts[3]};
                        }
                        else
                        {
                                treeDict[parts[0].TrimEnd(':')] = new Operation {Result = BigInteger.Parse(parts[1])};
                        }
                }

                var rootResult = ResolveNode("root", treeDict);

                // part2

                var rootLeftNode = treeDict["root"].Arg1;
                var rootRightNode = treeDict["root"].Arg2;

                var left = Resolve(rootLeftNode, treeDict);
                var right = Resolve(rootRightNode, treeDict);

                BigInteger humn = 0;

                if(left == null)
                {
                        humn = SolveForHumn(right.Value, rootLeftNode, treeDict);
                }
                else if(right == null)
                {
                        humn = SolveForHumn(left.Value, rootRightNode, treeDict);
                }



                Console.WriteLine( (rootResult.ToString(), humn.ToString()));
        }

        private BigInteger SolveForHumn(BigInteger target, string node, Dictionary<string, Operation> lookup)
        {
                // calculating node will give us target
                if(node == "humn")
                {
                        return target;
                }


                var ln = lookup[node].Arg1;
                var rn = lookup[node].Arg2;

                var lc = Resolve(ln, lookup);
                var rc = Resolve(rn, lookup);

                BigInteger tgt = 0;
                var op = lookup[node].Operator;

                if(lc == null)
                {
                        switch (op)
                        {
                                case "+":
                                        tgt = target - rc.Value;
                                        break;
                                case "-":
                                        tgt = target + rc.Value;
                                        break;
                                case "*":
                                        tgt = target / rc.Value;
                                        break;
                                case "/":
                                        tgt = target * rc.Value;
                                        break;
                                default:
                                        throw new Exception("bang");
                        }
                        return SolveForHumn(tgt, lookup[node].Arg1, lookup);
                }
                else
                {
                        switch (op)
                        {
                                case "+":
                                        tgt = target - lc.Value;
                                        break;
                                case "-":
                                        tgt = lc.Value - target;
                                        break;
                                case "*":
                                        tgt = target / lc.Value;
                                        break;
                                case "/":
                                        tgt = lc.Value / target;
                                        break;
                                default:
                                        throw new Exception("bang");
                        }
                        return SolveForHumn(tgt, lookup[node].Arg2, lookup);
                }

        }

        private BigInteger? Resolve(string node, Dictionary<string, Operation> lookup)
        {
                if(node == "humn")
                {
                        return null;
                }
                else
                {
                        var op = lookup[node];
                        if (op.Result != null)
                        {
                                return op.Result;
                        }
                        else
                        {
                                var left = Resolve(op.Arg1, lookup);
                                var right = Resolve(op.Arg2, lookup);
                                if(left == null || right==null)
                                {
                                        return null;
                                }
                                switch (op.Operator)
                                {
                                        case "+":
                                                op.Result = left + right;
                                                return op.Result;
                                        case "-":
                                                op.Result = left - right;
                                                return op.Result;
                                        case "*":
                                                op.Result = left * right;
                                                return op.Result;
                                        case "/":
                                                op.Result = left / right;
                                                return op.Result;
                                        default:
                                                throw new Exception("bang");
                                }
                        }
                }

        }



        private BigInteger ResolveNode(string node, Dictionary<string, Operation> lookup)
        {
                var op = lookup[node];
                if(op.Result != null)
                {
                        return (int)op.Result; 
                }
                else
                {
                        var left = ResolveNode(op.Arg1, lookup);
                        var right = ResolveNode(op.Arg2, lookup);
                        if(node == "root")
                        {
                                Console.WriteLine($"root: {left} {op.Operator} {right}");
                        }
                        switch(op.Operator)
                        {
                                case "+":
                                        return left + right;
                                case "-":
                                        return left - right;
                                case "*":
                                        return left * right;
                                case "/":
                                        return left / right;
                                default:
                                        throw new Exception("bang");
                        }
                }
        }

        private class Operation
        {
                public string Operator;
                public string Arg1;
                public string Arg2;
                public BigInteger? Result;
        }
}