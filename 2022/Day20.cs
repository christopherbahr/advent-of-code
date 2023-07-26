// Camp Cleanup
namespace AOC22;
public class Day20 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                long[] input = lines.Select(long.Parse).ToArray();
                LLNode[] origList = new LLNode[input.Length];
                LLNode[] p1OrigList = new LLNode[input.Length];

                LLNode head = null;
                LLNode prevNode = null;
                LLNode zeroNode = null;

                LLNode p1Head = null;
                LLNode p1PrevNode = null;
                LLNode p1ZeroNode = null;


                var key = 811589153;
                for(var i = 0; i<input.Length; i++)
                {
                        var num = input[i] * key;


                        var ll = new LLNode
                        {
                                Val = num,
                                Next = null,
                                Prev = prevNode
                        };
                        var p1ll = new LLNode
                        {
                                Val = input[i],
                                Next = null,
                                Prev = p1PrevNode
                        };

                        if(prevNode == null || p1PrevNode == null)
                        {
                                head = ll;
                                p1Head = p1ll;
                        }
                        else
                        {
                                prevNode.Next = ll;
                                p1PrevNode.Next = p1ll;
                        }

                        prevNode = ll;
                        p1PrevNode = p1ll;

                        origList[i] = ll;
                        p1OrigList[i] = p1ll;

                        if(num == 0)
                        {
                                zeroNode = ll;
                                p1ZeroNode = p1ll;
                        }
                }
                // Make LL circular
                prevNode.Next = head;
                head.Prev = prevNode;

                p1PrevNode.Next = p1Head;
                p1Head.Prev = p1PrevNode;

                long p1 = 0;
                //PrintList(p1ZeroNode, input.Length);

                for (int i = 0; i < input.Length; i++)
                {
                        var mn = p1OrigList[i];
                        MoveNode(mn, mn.Val, input.Length);
                        //PrintList(p1ZeroNode, input.Length);
                }

                var p1CountingNode = p1ZeroNode;
                for (int i = 0; i < 3001; i++)
                {
                        if (i % 1000 == 0 && i > 0)
                        {
                                p1 += p1CountingNode.Val;
                        }
                        p1CountingNode = p1CountingNode.Next;
                }
                //PrintList(zeroNode, input.Length);

                for (int j = 0; j < 10; j++)
                {
                        for (int i = 0; i < input.Length; i++)
                        {
                                var mn = origList[i];
                                MoveNode(mn, mn.Val, input.Length);
                                //PrintList(zeroNode, input.Length);
                        }
                }


                var countingNode =zeroNode;
                long score = 0;
                for(int i = 0; i<3001; i++)
                {
                        if(i % 1000 == 0 && i > 0)
                        {
                                score += countingNode.Val;
                        }
                        countingNode = countingNode.Next;
                }


                Console.WriteLine( (p1.ToString(), score.ToString()));
        }

        private void PrintList(LLNode startNode, int count)
        {
                var pn = startNode;
                for(int i = 0; i<count; i++)
                {
                        Console.Write(pn.Val);
                        Console.Write(", ");
                        pn= pn.Next;
                }
                Console.WriteLine();
        }

        private void MoveNode(LLNode node, long dist, long listLen)
        {
                dist = dist % (listLen - 1);

                if(Math.Abs(dist) > listLen / 2)
                {
                        if(dist > 0)
                        {
                                dist = dist - (listLen -1);
                        }
                        else
                        {
                                dist = (listLen-1) + dist;
                        }
                }
                if(dist > 0)
                {
                        var cn = node;
                        var tn = node;
                        for(var i = 0; i < dist; i++)
                        {
                                tn = cn.Next;
                                cn =tn;
                        }

                        node.Prev.Next = node.Next;
                        node.Next.Prev = node.Prev;

                        var nn = tn.Next;

                        tn.Next = node;
                        node.Prev = tn;
                        node.Next = nn;
                        nn.Prev = node;
                }
                else if(dist < 0)
                {
                        var cn = node;
                        var tn = node;
                        for(var i = 0; i < Math.Abs(dist); i++)
                        {
                                tn = cn.Prev;
                                cn = tn;
                        }

                        node.Prev.Next = node.Next;
                        node.Next.Prev = node.Prev;

                        var pp = tn.Prev;

                        tn.Prev = node;
                        node.Next = tn;
                        pp.Next = node;
                        node.Prev = pp;
                }

        }

        private class LLNode
        {
                public long Val;
                public LLNode? Next;
                public LLNode? Prev;
        }
}