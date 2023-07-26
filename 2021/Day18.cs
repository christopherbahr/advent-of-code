// the original implementation of this dynamically generated the ordered list 
// of nodes that are actual numbers everyTime it needed to do an explode.
// It would discard this list after each explode
// This salved the whole thing in about a second and a half
// The improvement to treat maintain both the ordered list and the 
// tree structure got it down to about 40ms

// I also changed everything to prefer the ordered list to the 
// tree for browsing. That was probably unneccesary, the trick I thnik
// is not rebuilding the list all the time. It could equally browse by the tree
// and be similarly fast. Maintianing the depth information on the list is nice though.

// Another improvement would be to generate the ordered list for each number and then 
// just copy them every time we need a new one.
//
// Result: That's really hard because you have to copy the lists to get things working nicely
// you would have to change the way relationships between entities are serialized to be something like
// offset in an array.
//
// You could definitely do it and it would be faster but for now I'm going to leave it alone

namespace AOC21;
public class Day18 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var fullEntry = ParseInput(lines.First(), 0);
                foreach(var line in lines.Skip(1))
                {
                        var e = ParseInput(line, 0);
                        fullEntry = Add(fullEntry, e);
                        var ol = GetOrderedList(fullEntry, false);
                        ProcessOL(fullEntry, ol);
                }
                Console.WriteLine(Magnitude(fullEntry, 1));

                // Reparse because everything gets mutated
                var entries = new List<Entry>();
                foreach(var line in lines)
                {
                        var e = ParseInput(line, 0);
                        entries.Add(e);
                }

                int biggest = 0;
                foreach(var (i, a) in entries.Enumerate())
                {
                        foreach (var (j, b) in entries.Enumerate())
                        {
                                if (a == b)
                                {
                                        continue;
                                }
                                // copy so we don't mess them up
                                var ua = DeepCopy(a);
                                var ub = DeepCopy(b);
                                var e1 = Add(ua, ub);
                                var ol = GetOrderedList(e1, false);
                                ProcessOL(e1, ol.ToList());
                                var v = Magnitude(e1, 1);
                                if (v > biggest)
                                {
                                        biggest = v;
                                }
                        }
                }
                Console.WriteLine(biggest);
        }

        private int Magnitude(Entry e, int mul)
        {
                int mag = 0;
                if(e.lv.HasValue)
                {
                        mag += (3 * mul * e.lv.Value);
                }
                else
                {
                        mag += Magnitude(e.left, 3 * mul);
                }

                if(e.rv.HasValue)
                {
                        mag += (2 * mul * e.rv.Value);
                }
                else
                {
                        mag += Magnitude(e.right, 2 * mul);
                }

                return mag;
        }

        private Entry? DeepCopy(Entry e)
        {
                if(e == null)
                {
                        return null;
                }
                var ne = new Entry
                {
                        lv = e.lv,
                        rv = e.rv,
                        right = DeepCopy(e.right),
                        left = DeepCopy(e.left)
                };
                if(ne.right != null)
                {
                        ne.right.parent = ne;
                }
                if(ne.left != null)
                {
                        ne.left.parent = ne;
                }
                return ne;
        }

        private Entry Add(Entry a, Entry b)
        {
                var ne = new Entry();
                ne.left = a;
                ne.right = b;
                a.parent = ne;
                b.parent = ne;
                return ne;
        }

        private void ProcessOL(Entry root, List<(Entry e, bool l, int d)> ol)
        {
                var explode = true;
                var split = true;
                // this isn't as simple as possible but 
                // it avoids trying to explode again after a fialed explode
                while(explode || split)
                {
                        //Console.WriteLine(PrettyPrint(root));
                        if (explode)
                        {
                                var exploded = ExplodeOL(ol);
                                if (exploded)
                                {
                                        explode = true;
                                        split = true;
                                }
                                else
                                {
                                        explode = false;
                                }
                        }
                        if(split && !explode)
                        {
                                var didSplit = SplitOL(ol);
                                if(didSplit)
                                {
                                        explode = true;
                                        split = true;
                                }
                                else
                                {
                                        split = false;
                                        break;
                                }
                        }
                }
        }

        private bool SplitOL(List<(Entry e, bool l, int d)> ol)
        {
                var split = false;
                (Entry, bool, int) addedEntry  = (null, false, 0);
                (Entry, bool, int) addedEntry2  = (null, false, 0);
                var removeIdx = -1;
                foreach(var (i, le) in ol.Enumerate())
                {
                        if(le.l && le.e.lv > 9)
                        {
                                var ne = new Entry();
                                ne.parent = le.e;
                                ne.lv = le.e.lv / 2;
                                ne.rv = (le.e.lv / 2);
                                if(le.e.lv % 2 != 0)
                                {
                                        ne.rv += 1;
                                }
                                le.e.left = ne;
                                le.e.lv = null;
                                split = true;
                                addedEntry = (ne, true, le.d+1);
                                addedEntry2 = (ne, false, le.d+1);
                                removeIdx = i;
                                break;
                        }
                        else if(!le.l && le.e.rv > 9)
                        {
                                var ne = new Entry();
                                ne.parent = le.e;
                                ne.lv = le.e.rv / 2;
                                ne.rv = (le.e.rv / 2);
                                if(le.e.rv % 2 != 0)
                                {
                                        ne.rv += 1;
                                }
                                le.e.right = ne;
                                le.e.rv = null;
                                split = true;
                                addedEntry = (ne, true, le.d+1);
                                addedEntry2 = (ne, false, le.d+1);
                                removeIdx = i;
                                break;
                        }
                }
                if(split)
                {
                        ol.RemoveAt(removeIdx);
                        ol.Insert(removeIdx, addedEntry2);
                        ol.Insert(removeIdx, addedEntry);
                }

                return split;
        }

        private List<(Entry e, bool l, int d)> GetOrderedList(Entry root, bool increaseDepth)
        {

                var cn = root;
                var ol = new List<(Entry e, bool l, int d)>();
                var v = new HashSet<Entry>();
                var d = increaseDepth ? 1 : 0;
                while (true)
                {
                        if(cn.lv.HasValue && !v.Contains(cn) && !ol.Contains((cn, true, d)))
                        {
                                ol.Add((cn, true, d));
                        }

                        if(cn.left != null && !v.Contains(cn.left))
                        {
                                cn = cn.left;
                                d++;
                                continue;
                        }
                        if(cn.right != null && !v.Contains(cn.right))
                        {
                                cn = cn.right;
                                d++;
                                continue;
                        }
                        if(cn.rv.HasValue && !v.Contains(cn) && !ol.Contains((cn, false, d)))
                        {
                                ol.Add((cn, false, d));
                        }
                        if(cn.parent != null)
                        {
                                v.Add(cn);
                                d--;
                                cn = cn.parent;
                        }
                        else
                        {
                                // we made back to the root
                                break;
                        }
                }

                return ol;
        }


        private bool ExplodeOL(List<(Entry e, bool l, int d)> ol)
        {
                var enidx = ol.FindIndex(x=> x.d >= 4);
                if(enidx == -1)
                {
                        return false;
                }
                var (en, enl, end) = ol[enidx];
                if(enidx != 0)
                {
                        var (pn, l, d) = ol[enidx - 1];
                        if(l)
                        {
                                pn.lv += en.lv;
                        }
                        else
                        {
                                pn.rv += en.lv;
                        }
                }
                if(enidx != ol.Count - 2)
                {
                        //Console.WriteLine($"{enidx} {ol.Count}");
                        var (nn, l, d) = ol[enidx + 2];
                        if(l)
                        {
                                nn.lv += en.rv;
                        }
                        else
                        {
                                nn.rv += en.rv;
                        }
                }

                var toAdd = (en.parent, enl, end - 1);
                if(en.parent.right == en)
                {
                        en.parent.right = null;
                        en.parent.rv = 0;
                        toAdd.enl = false;
                }
                else
                {
                        en.parent.left = null;
                        en.parent.lv = 0;
                        toAdd.enl = true;
                }
                // Remove the left and right entries in the ordered list
                ol.RemoveAt(enidx);
                ol.RemoveAt(enidx);

                ol.Insert(enidx, toAdd);

                return true;
        }

        private string PrettyPrint(Entry entry)
        {
                var retStr = "[";
                if(entry.lv.HasValue)
                {
                        retStr += entry.lv;
                }
                else
                {
                        retStr += PrettyPrint(entry.left);
                }
                retStr += ",";
                if(entry.rv.HasValue)
                {
                        retStr += entry.rv;
                }
                else
                {
                        retStr += PrettyPrint(entry.right);
                }
                return retStr + "]";
        }

        private Entry ParseInput(string input, int sp)
        {
                var e = new Entry();
                var (c, mb) = FindMatchingBracket(input, sp);
                if(input[sp+1] == '[')
                {
                        e.left = ParseInput(input, sp + 1);
                        e.left.parent = e;
                }
                else
                {
                        e.lv = int.Parse(input.Substring(sp + 1, 1));
                }
                if(input[c+1] == '[')
                {
                        e.right = ParseInput(input, c + 1);
                        e.right.parent = e;
                }
                else
                {
                        e.rv = int.Parse(input.Substring(c+1, 1));
                }
                return e;
        }

        private (int c, int b) FindMatchingBracket(string input, int pos)
        {
                var count = 1;
                pos++;
                var comma = 0;
                while(count != 0)
                {
                        var c = input[pos];
                        if(c == '[')
                        {
                                count++;
                        }
                        else if(c == ']')
                        {
                                count--;
                        }
                        else if(c == ',' && count == 1)
                        {
                                comma = pos;
                        }
                        pos++;
                }
                return (comma, pos);
        }

        private class Entry
        {
                public Entry? left;
                public Entry? right;
                public Entry? parent;
                public int? lv;
                public int? rv;
        }
}