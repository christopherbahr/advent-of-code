namespace AOC22;
public class Day13 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var packetPairs = new List<(Packet?, Packet?)>();

                Packet? p1 = null;
                Packet? p2 = null;
                foreach(var line in lines)
                {
                        if(!line.Any())
                        {
                                packetPairs.Add((p1, p2));
                                p1 = null; 
                                p2 = null;
                                continue;
                        }
                        var packet = ParsePacket(line);
                        if(p1 == null)
                        {
                                p1 = packet;
                        }
                        else
                        {
                                p2 = packet;
                        }
                }
                packetPairs.Add((p1, p2));

                var idxList = new List<int>();

                var sum = 0;
                // now compare
                for(var i = 0; i < packetPairs.Count(); i++)
                {
                        var (pp1, pp2) = packetPairs[i];
                        var e1 = new Entry { Entries = pp1.PacketChunks};
                        var e2 = new Entry { Entries = pp2.PacketChunks};
                        if(CompareEntries2(e1, e2) == Status.Success)
                        {
                                idxList.Add(i+ 1);
                                sum += (i + 1);
                        }
                }

                Console.WriteLine(sum);

                var allPackets = packetPairs.SelectMany(x => new[] {x.Item1, x.Item2}).ToList();
                 
                allPackets.Sort(new PacketComparer());

                var firstIdx = -1;
                var secondIdx = -1;
                for(var i = 0; i< allPackets.Count; i++)
                {
                        if(allPackets[i].IsDivider)
                        {
                                if(firstIdx == -1)
                                {
                                        firstIdx = i;
                                }
                                else
                                {
                                        secondIdx = i;
                                }
                        }
                }

                var p2Answer = (firstIdx + 1) * (secondIdx+1);


                Console.WriteLine( (sum.ToString(), p2Answer.ToString()));
        }

        private class PacketComparer : IComparer<Packet?>
        {
                public int Compare(Packet? x, Packet? y)
                {
                        if(x == null || y== null)
                        {
                                throw new Exception("Null packet");
                        }
                        var e1 = new Entry { Entries = x.PacketChunks};
                        var e2 = new Entry { Entries = y.PacketChunks};
                        var status = CompareEntries2(e1, e2);
                        return status == Status.Success ? -1 : 1;
                }
        }

        public enum Status
        {
                Success, 
                Failure,
                Unknown
        }

        public static Status CompareEntries2(Entry e1, Entry e2)
        {
                if (e1.Value != -1 && e2.Value != -1)
                {
                        if(e1.Value < e2.Value)
                        {
                                return Status.Success;
                        }
                        else if(e1.Value > e2.Value)
                        {
                                return Status.Failure;
                        }
                        else
                        {
                                return Status.Unknown;
                        }
                }
                else if(e1.Value == -1 && e2.Value == -1)
                {
                        var l1 = e1.Entries;
                        var l2 = e2.Entries;
                        for (int i = 0; i < l1.Count(); i++)
                        {
                                if(i >= l2.Count())
                                {
                                        return Status.Failure;
                                }
                                var status = CompareEntries2(l1[i], l2[i]);
                                if(status == Status.Success || status == Status.Failure)
                                {
                                        return status;
                                }
                        }
                        // We compared all the data we had and apparently got unknown
                        // Let's check if the left side ran out first
                        if(l1.Count() < l2.Count)
                        {
                                return Status.Success;
                        }
                        return Status.Unknown;
                }
                //e1 is a value but e2 is a list
                else if(e1.Value >= 0)
                {
                        var te = new Entry { Entries = new List<Entry>{e1}};
                        var status = CompareEntries2(te, e2);
                        return status;
                }
                else
                {
                        var te = new Entry { Entries = new List<Entry>{e2}};
                        var status = CompareEntries2(e1, te);
                        return status;
                }


        }


        public (bool res, bool cont) CompareEntries(Entry e1, Entry e2)
        {
                if (e1.Value != -1 && e2.Value != -1)
                {
                        if(e1.Value < e2.Value)
                        {
                                return (true, false);
                        }
                        if(e1.Value > e2.Value)
                        {
                                return (false, false);
                        }
                        if(e1.Value == e2.Value)
                        {
                                return (true, true);
                        }
                }
                else if(e1.Value == -1 && e2.Value == -1)
                {
                        var l1 = e1.Entries;
                        var l2 = e2.Entries;
                        for (int i = 0; i < l1.Count(); i++)
                        {
                                if(i >= l2.Count())
                                {
                                        return (false, false);
                                }
                                var (resres, rescont) = CompareEntries(l1[i], l2[i]);
                                if(!rescont)
                                {
                                        return (resres, rescont);
                                }
                        }
                }
                //e1 is a value but e2 is a list
                else if(e1.Value >= 0)
                {
                        var te = new Entry { Entries = new List<Entry>{e1}};
                        var (resres, rescont) = CompareEntries(te, e2);
                        if (!rescont)
                        {
                                return (resres, rescont);
                        }
                }
                // e2 iS a value and e1 is a list
                else
                {
                        var te = new Entry { Entries = new List<Entry>{e2}};
                        var (resres, rescont) = CompareEntries(e1, te);
                        if (!rescont)
                        {
                                return (resres, rescont);
                        }
                }
                return (true, false);

        }

        public int FindMatchingBracket(string s, int startIdx)
        {
                var depth = 1;
                for(var i = startIdx + 1; i < s.Length; i++)
                {
                        if(s[i] == '[')
                        {
                                depth++;
                        }
                        else if(s[i] == ']')
                        {
                                depth--;
                                if(depth == 0)
                                {
                                        return i;
                                }
                        }
                }
                return -1;
        }

        public Packet ParsePacket(string s)
        {
                s = new string(s.Skip(1).SkipLast(1).ToArray());
                var packet = new Packet();
                var i = 0;
                while(i < s.Length)
                {
                        var c = s[i];
                        if(c == '[')
                        {
                                var match = FindMatchingBracket(s, i);
                                var packetString = s.Substring(i, match-i+1);
                                var subPacket = ParsePacket(packetString);
                                packet.PacketChunks.Add(new Entry {Entries = subPacket.PacketChunks});
                                i = match +1;

                        }
                        else if(c == ',')
                        {
                                i++;
                                continue;
                        }
                        else if(c == ']')
                        {
                                i++;
                                Console.WriteLine("Found unexpected end token, trying to continue");
                        }
                        else // parse the next int
                        {
                                var nextComma = s.IndexOf(',', i);
                                var val = nextComma == -1 ? int.Parse(s.Substring(i)) : int.Parse(s.Substring(i, nextComma-i));
                                packet.PacketChunks.Add(new Entry {Value = val});
                                i = nextComma+1;
                                if(nextComma == -1)
                                {
                                        break;
                                }
                        }
                }
                return packet;
        }
        
        public class Packet
        {
                public List<Entry> PacketChunks = new List<Entry>();

                public bool IsDivider => PacketChunks.Count() == 1 && PacketChunks.Single().Entries?.Count() == 1
                && (PacketChunks.Single().Entries?.Single().Value == 2 || PacketChunks.Single().Entries?.Single().Value == 6);
        }

        public class Entry
        {
                public int Value {get; set;} = -1;
                public List<Entry>? Entries { get; set; }
        }
}