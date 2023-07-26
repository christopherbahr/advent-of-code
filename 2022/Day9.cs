
namespace AOC22;
public class Day9 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var shortField = new HashSet<(int, int)>();
                var longField = new HashSet<(int, int)>();

                var shortRope = new List<(int, int)> { (0, 0), (0, 0) };
                var longRope = new List<(int, int)>();

                foreach(var i in Enumerable.Range(0, 10))
                {
                        longRope.Add((0, 0));
                }

                var ropes = new [] {(shortRope, shortField), (longRope, longField)};

                foreach(var line in lines)
                {
                        var instruction = line.Split(' ');
                        var count = int.Parse(instruction[1]);
                        var dr = 0;
                        var dc = 0;
                        switch(instruction[0])
                        {
                                case "U":
                                        dr = 1;
                                        break;
                                case "D":
                                        dr = -1;
                                        break;
                                case "R":
                                        dc = 1;
                                        break;
                                case "L":
                                        dc = -1;
                                        break;
                        }


                        foreach (var (rope, field) in ropes)
                        {
                                for (int i = 0; i < count; i++)
                                {
                                        rope[0] = (rope[0].Item1 + dr, rope[0].Item2 + dc);
                                        var head = rope[0];
                                        var cr = head.Item1;
                                        var cc = head.Item2;
                                        for (int j = 1; j < rope.Count; j++)
                                        {
                                                rope[j] = UpdateTail(cr, cc, rope[j].Item1, rope[j].Item2);
                                                cr = rope[j].Item1;
                                                cc = rope[j].Item2;
                                        }
                                        field.Add(rope.Last());
                                }
                        }
                               
                }


                Console.WriteLine( (shortField.Count().ToString(), longField.Count().ToString()));
        }

        public (int, int) UpdateTail(int hr, int hc, int tr, int tc)
        {
                if(Math.Abs(hr - tr) <= 1 && Math.Abs(hc - tc) <= 1)
                {
                        // no need to move we are within one
                        return (tr, tc);
                }

                if(hr == tr)
                {
                        if(hc > tc)
                        {
                                tc++;
                        }
                        else
                        {
                                tc--;
                        }
                }
                else if(hc == tc)
                {
                        if(hr > tr)
                        {
                                tr++;
                        }
                        else
                        {
                                tr--;
                        }
                }
                else
                {
                        if(hr > tr)
                        {
                                tr++;
                        }
                        else
                        {
                                tr--;
                        }
                        if(hc > tc)
                        {
                                tc++;
                        }
                        else
                        {
                                tc--;
                        }
                }
                return (tr, tc);
        }
}