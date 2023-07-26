using System.Numerics;

namespace AOC22;
public class Day25 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                Console.WriteLine("25");

                BigInteger counter = 0;

                foreach(var line in lines)
                {
                        var snafuTrans = SnafuTranslation(line);
                        var redec = DecimalToSnafu(snafuTrans);
                        counter += snafuTrans;
                        Console.WriteLine(line + " | " + snafuTrans + " | " + redec);
                }

                Console.WriteLine(counter);
                var snafudCount = DecimalToSnafu(counter);
                Console.WriteLine(snafudCount);
                
                Console.WriteLine( (snafudCount.ToString(), "MERRY CHRISTMAS"));
        }

        public Dictionary<int, (int, int)> ranges = new Dictionary<int, (int, int)>();

        public (BigInteger min, BigInteger max) GetRangeForPower(int power)
        {
                if(power < 0)
                {
                        return (0, 0);
                }
                if(power == 0)
                {
                        return (-2, 2);
                }
                if(ranges.ContainsKey(power))
                {
                         return ranges[power];
                }
                var pow = (BigInteger) Math.Pow(5, power);
                var prevPow = GetRangeForPower(power - 1);
                var max = prevPow.max + 2 * pow;
                var min = prevPow.min - 2 * pow;
                return (min, max);
        }

        public (BigInteger val, char snafu) NextSnafu(BigInteger dec, int pow)
        {
                var range = GetRangeForPower(pow);
                var prevRange = GetRangeForPower(pow - 1);
                var exp = (BigInteger)Math.Pow(5, pow);
                if(dec == 0)
                {
                        return (0, '0');
                }
                if((dec > 0 && exp + prevRange.min > dec) || (dec < 0 && -exp + prevRange.max < dec))
                {
                        return (0, '0');
                }
                if(dec > 0)
                {
                        // 1 + max is to small, we'll need to use 2
                        if(exp + prevRange.max < dec)
                        {
                                return (2 * exp, '2');
                        }
                        else
                        {
                                return (exp, '1');
                        }
                }
                else
                {
                        if(-exp - prevRange.max > dec)
                        {
                                return (-2 * exp, '=');
                        }
                        else
                        {
                                return (-exp, '-');
                        }
                }

        }

        public string DecimalToSnafu(BigInteger dec)
        {
                var pow = 0;
                while(true)
                {
                        var range = GetRangeForPower(pow);
                        if(dec > range.max || dec < range.min )
                        {
                                pow++;
                                continue;
                        }
                        else
                        {
                                break;
                        }
                }
                var response = "";
                var decLeft = dec;

                var counter = 0;
                for(int i = pow; i >= 0; i--)
                {
                        var (decrement, nextChar) = NextSnafu(decLeft, pow - counter);
                        response += nextChar;
                        decLeft = decLeft - decrement;
                        counter++;
                }
                return response;
        }

        public BigInteger SnafuTranslation(string snafu)
        {
                var inOrder = snafu.Reverse().ToList();

                BigInteger total = 0;

                for(var i = 0; i < inOrder.Count; i++)
                {
                        var exp = (BigInteger)Math.Pow(5, i);
                        var val = 0;
                        switch(inOrder[i])
                        {
                                case '2':
                                        val = 2;
                                        break;
                                case '1':
                                        val = 1;
                                        break;
                                case '0':
                                        val = 0;
                                        break;
                                case '-':
                                        val = -1;
                                        break;
                                case '=':
                                        val = -2;
                                        break;
                        }

                        var toAdd = val * exp;
                        total += toAdd;
                }

                return total;
        }
}