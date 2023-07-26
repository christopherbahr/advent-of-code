using System.Collections;
using System.Globalization;
using System.Numerics;

namespace AOC21;
public class Day16 : IDay
{
        public string inputFile {get; set;} = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var line = lines.First();
                var bits = GetBits(line);
                var (p, _) = GetNextPacket(bits, 0);
                //PrettyPrint(p,"");
                var verSum = SumVersions(p);
                Console.WriteLine(verSum);
                var val = EvaluatePacket(p);
                Console.WriteLine(val);
        }

        private BigInteger EvaluatePacket(Packet packet)
        {
                BigInteger v = long.MinValue;
                switch(packet.Type)
                {
                        case 0:
                                v = 0;
                                foreach(var p in packet.SubPackets)
                                {
                                        v += EvaluatePacket(p);
                                }
                                break;
                        case 1: 
                                v = 1;
                                foreach(var p in packet.SubPackets)
                                {
                                         v*= EvaluatePacket(p);
                                }
                                break;
                        case 2:
                                v = long.MaxValue;
                                foreach(var p in packet.SubPackets)
                                {
                                        var r = EvaluatePacket(p);
                                        if(r < v)
                                        {
                                                v = r;
                                        }
                                }
                                break;
                        case 3:
                                v = long.MinValue;
                                foreach(var p in packet.SubPackets)
                                {
                                        var r = EvaluatePacket(p);
                                        if(r > v)
                                        {
                                                v = r;
                                        }
                                }
                                break;
                        case 4:
                                v = packet.Value;
                                break;
                        case 5:
                                var gl = EvaluatePacket(packet.SubPackets.First());
                                var gr = EvaluatePacket(packet.SubPackets.Skip(1).First());
                                v = gl > gr ? 1 : 0;
                                break;
                        case 6:
                                var ll = EvaluatePacket(packet.SubPackets.First());
                                var lr = EvaluatePacket(packet.SubPackets.Skip(1).First());
                                v = ll < lr ? 1 : 0;
                                break;
                        case 7:
                                var el = EvaluatePacket(packet.SubPackets.First());
                                var er = EvaluatePacket(packet.SubPackets.Skip(1).First());
                                v = el == er ? 1 : 0;
                                break;
                        default:
                                throw new Exception("bang!");
                }
                return v;
        }

        private int SumVersions(Packet packet)
        {
                var ver = packet.Version;
                foreach(var p in packet.SubPackets)
                {
                        ver += SumVersions(p);
                }
                return ver;
        }

        private void PrettyPrint(Packet packet, string offset)
        {
                var pktStr = $"{offset} {packet.Version} {packet.Type} ";
                if(packet.Type == 4)
                {
                        pktStr += packet.Value;
                        Console.WriteLine(pktStr);
                }
                else
                {
                        pktStr += packet.Mode;
                        Console.WriteLine(pktStr);
                        foreach(var p in packet.SubPackets)
                        {
                                PrettyPrint(p, offset + "    ");
                        }
                }
        }

        private (Packet packet, int pointer) GetNextPacket(BitArray bits, int offset)
        {

                var p = new Packet();
                var curPointer = offset;

                var ver = 0;
                ver += bits.Get(curPointer) ? 1 << 2 : 0;
                ver += bits.Get(curPointer + 1) ? 1 << 1 : 0;
                ver += bits.Get(curPointer + 2) ? 1 : 0;
                curPointer+= 3;

                var type = 0;
                type += bits.Get(curPointer) ? 1 << 2 : 0;
                type += bits.Get(curPointer + 1) ? 1 << 1 : 0;
                type += bits.Get(curPointer + 2) ? 1 : 0;
                curPointer+=3;

                p.Version = ver;
                p.Type = type;

                // Literal
                if(p.Type == 4)
                {
                        long val = 0;
                        // how many chunks is the number
                        var len = 0;
                        var bc = 0;
                        while(true)
                        {
                                len++;
                                if(!bits.Get(bc + curPointer))
                                {
                                        break;
                                }
                                bc+= 5;
                        }
                        //Console.WriteLine(len);
                        for(var i = 0; i<len; i++)
                        {
                                for(var j = 0; j < 4; j++)
                                {
                                        var idx = 1 + (5 * i) + j + curPointer;
                                        var shift = (len * 4) - ((4 * i) + j) - 1;
                                        // offset is totalPackets * 4 - (4i + j)

                                        val += bits.Get(idx) ? 1L << shift : 0;
                                        //Console.WriteLine($"{val} {idx} {offset} {bits.Get(idx)}");
                                }
                        }
                        p.Value = val;
                        curPointer += len * 5;
                        //Console.WriteLine($"{p.Version} {p.Type} {p.Value} {curPointer}");
                }
                else
                {
                        var mode = bits.Get(curPointer);
                        p.Mode = mode;
                        curPointer++;
                        if(mode)
                        {
                                // Number of sub packets immediately contained
                                var packetCount = 0;
                                for(var i = 0; i < 11; i++, curPointer++)
                                {
                                        if(bits.Get(curPointer))
                                        {
                                                packetCount += 1 << 10 - i;
                                        }
                                }
                                //Console.WriteLine($"{p.Version} {p.Type} {packetCount}");
                                for(var i = 0; i < packetCount; i++)
                                {
                                        // Double check this
                                        var (np, o) = GetNextPacket(bits, curPointer);
                                        p.SubPackets.Add(np);
                                        curPointer = o;
                                }

                        }
                        else
                        {
                                // total length of sub-packets
                                var pLength = 0;
                                for(var i = 0; i < 15; i++, curPointer++)
                                {
                                        if(bits.Get(curPointer))
                                        {
                                                pLength += 1 << 14 - i;
                                        }
                                }
                                var targetLength = curPointer + pLength;
                                //Console.WriteLine($"{p.Version} {p.Type} {pLength} {curPointer}");

                                while(curPointer < targetLength)
                                {
                                        var (np, o) = GetNextPacket(bits, curPointer);
                                        p.SubPackets.Add(np);
                                        curPointer = o;
                                }
                        }

                }

                return (p, curPointer);
        }

        private BitArray GetBits(string packetString)
        {
                var ba = new BitArray(4 * packetString.Length);
                foreach(var (i, c) in packetString.Enumerate())
                {
                        var b = byte.Parse(new [] { c }, NumberStyles.HexNumber);
                        for(var j = 0; j < 4; j++)
                        {
                                ba.Set(i * 4 + j, (b & (1 << (3 - j))) != 0);
                        }
                }
                //Console.WriteLine(packetString);
                PrettyPrint(ba);
                return ba;
        }

        private void PrettyPrint(BitArray ba)
        {
                var str = "";
                for(var i = 0; i < ba.Count; i++)
                {
                        str += ba.Get(i) ? 1 : 0;
                }
                //Console.WriteLine(str);
        }


        private class Packet
        {
                public int Version;
                public int Type;
                public long Value;
                public bool Mode;
                public List<Packet> SubPackets = new List<Packet>();
        }
}