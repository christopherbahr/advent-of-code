using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class Helpers
{
        public static IEnumerable<(int i, T val)> Enumerate<T>(this IEnumerable<T> input)
        {
                var i = 0;
                foreach(var thing in input)
                {
                        yield return (i, thing);
                        i++;
                }
        }

        public static IEnumerable<((int r, int c) p, T v)> Enumerate<T>(this T[,] grid)
        {
                for(var i = 0; i < grid.GetLength(0); i++)
                {
                        for(var j = 0; j < grid.GetLength(1); j++)
                        {
                                yield return ((i, j), grid[i,j]);
                        }
                }
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> input, T match)
        {
                return Split(input, x => x?.Equals(match) ?? false);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> input, Func<T, bool> predicate)
        {
                var workingEnum = new List<T>();
                foreach(var thing in input)
                {
                        if(predicate(thing))
                        {
                                yield return workingEnum;
                                workingEnum = new List<T>();
                                continue;
                        }
                        else
                        {
                                workingEnum.Add(thing);
                        }
                }
                yield return workingEnum;
        }

        public static char[,] AsCharGrid(this IEnumerable<string> input)
        {
                var cg = new char[input.Count(), input.First().Count()];
                foreach(var e in CharGrid(input) )
                {
                        cg[e.p.r, e.p.c] = e.ch;
                }
                return cg;
        }

        public static IEnumerable<((int r, int c) p, char ch)> CharGrid(this IEnumerable<string> input)
        {
                foreach(var (r, str) in input.Enumerate())
                {
                        foreach(var (c, ch) in str.Enumerate())
                        {
                                yield return ((r, c), ch);
                        }
                }
        }

        public static int[,] AsIntGrid(this IEnumerable<string> input)
        {
                var g = new int[input.Count(), input.First().Count()];
                foreach(var e in IntGrid(input) )
                {
                        g[e.p.r, e.p.c] = e.v;
                }
                return g;
        }

        public static IEnumerable<((int r, int c) p, int v)> IntGrid(this IEnumerable<string> input)
        {
                foreach(var (r, str) in input.Enumerate())
                {
                        foreach(var (c, ch) in str.Enumerate())
                        {
                                yield return ((r, c), ch - '0');
                        }
                }
        }

        public static IEnumerable<(int r, int c)> GridIter(int rowCount, int colCount)
        {
                for(var i = 0; i < rowCount; i++)
                {
                        for(var j = 0; j < colCount; j++)
                        {
                                yield return (i, j);
                        }
                }
        }

        /// <summary>
        /// Useful for comparing every element against every other element in a list.
        /// Insists on the list so that we can have random access though it would b e
        /// possible to do this based on multiple enumerators without too much difficulty
        /// in practice it's almost always fine to just use a list.
        /// Note: This will not return the items on the diagonal. So you wont compare the
        /// first against the first ever.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<(T v1, T v2)> TriangleIter<T>(this IList<T> input)
        {
                foreach(var i in Enumerable.Range(0, input.Count))
                {
                        foreach(var j in Enumerable.Range(i + 1, input.Count - i - 1))
                        {
                                yield return (input[i], input[j]);
                        }
                }
        }

        public static void PrettyPrint<T>(this T[,] grid)
                where T : notnull
        {
                for(var i = 0; i < grid.GetLength(0); i++)
                {
                        var sb = "";
                        for(var j = 0; j < grid.GetLength(1); j++)
                        {
                                sb += grid[i,j]?.ToString();
                        }
                        Console.WriteLine(sb);
                }
        }

        public static void PrettyPrint(this HashSet<(int, int)> grid)
        {
                var minr = grid.Min(x => x.Item1);
                var maxr = grid.Max(x => x.Item1);
                var minc = grid.Min(x => x.Item2);
                var maxc = grid.Max(x => x.Item2);
                for(var cr = minr; cr <= maxr; cr++)
                {
                        var str = "";
                        for(var cc = minc; cc <= maxc; cc++)
                        {
                                if(grid.Contains((cr, cc)))
                                {
                                        str += '#';
                                }
                                else
                                {
                                        str += '.';
                                }
                        }
                        Console.WriteLine (str);
                }
        }

        public static bool ContainsPoint<T>(this T[,] grid, int row, int column)
        {
                var rowCount = grid.GetLength(0);
                var colCount = grid.GetLength(1);
                return row >= 0 && row < rowCount && column >= 0 && column < colCount;
        }

        public static (int, int) North = (-1, 0);
        public static (int, int) NorthWest = (-1, -1);
        public static (int, int) NorthEast = (-1, 1);
        public static (int, int) South = (1, 0);
        public static (int, int) SouthWest = (1, -1);
        public static (int, int) SouthEast = (1, 1);
        public static (int, int) East = (0, 1);
        public static (int, int) West = (0, -1);
        public static (int, int) Center = (0, 0);


        public static IEnumerable<(int cr, int cc)> cDirs(this (int r, int c) current)
        {
                foreach(var (dr, dc) in rawCDirs)
                {
                        yield return (current.r + dr, current.c + dc);
                }
        }

        public static IEnumerable<(int cr, int cc)> allDirs(this (int r, int c) current)
        {
                foreach(var (dr, dc) in rawAllDirs)
                {
                        yield return (current.r + dr, current.c + dc);
                }
        }

        public static IEnumerable<(int cr, int cc)> cDirs<T>(this (int r, int c) current, T[,] grid)
        {
                return current.cDirs().Where(x => grid.ContainsPoint(x.cr, x.cc));
        }

        public static IEnumerable<(int cr, int cc)> allDirs<T>(this (int r, int c) current, T[,] grid)
        {
                return current.allDirs().Where(x => grid.ContainsPoint(x.cr, x.cc));
        }

        public static (int, int)[] rawCDirs = new[] {North, South, East, West};
        public static (int, int)[] rawAllDirs = new[] {North, NorthWest, NorthEast, South, SouthWest, SouthEast,East, West};

        public static int ManhattanDistance(this (int r, int c) p, (int r, int c) op)
        {
                return Math.Abs(p.r - op.r) + Math.Abs(p.c - op.c);
        } 

        public static U GetOrAdd<T, U>(this Dictionary<T, U> dict, T key,  Func<U> factory)
                where T : notnull
        {
                if(!dict.TryGetValue(key, out var val))
                {
                        var newVal = factory.Invoke();
                        dict[key] = newVal;
                        return newVal;
                }
                return val;
        }

        public static void AddToList<T, U>(this Dictionary<T, IList<U>> dict, T key, U entry)
                where T : notnull
        {
                if(!dict.TryGetValue(key, out var val))
                {
                        dict[key] = new List<U> { entry };
                }
                else
                {
                        val.Add(entry);
                }
        }

        public static void AddToVal<T>(this Dictionary<T, int> dict, T key, int v)
                where T : notnull
        {
                if(dict.ContainsKey(key))
                {
                        dict[key] += v;
                }
                else
                {
                        dict[key] = v;
                }
        }

        public static void AddToVal<T>(this Dictionary<T, long> dict, T key, long v)
                where T : notnull
        {
                if(dict.ContainsKey(key))
                {
                        dict[key] += v;
                }
                else
                {
                        dict[key] = v;
                }
        }

        public static void AddToVal<T>(this Dictionary<T, BigInteger> dict, T key, BigInteger v)
                where T : notnull
        {
                if(dict.ContainsKey(key))
                {
                        dict[key] += v;
                }
                else
                {
                        dict[key] = v;
                }
        }

        public static List<int> GetInts(this string input)
        {
                var r = new Regex(@"(-?\d+)");
                var matches = r.Matches(input);

                return matches.SelectMany(x => x.Captures)
                                  .Select(x => x.Value)
                                  .Select(x => { var p = int.TryParse(x, out var v); return(p, v);} )
                                  .Where(x => x.p)
                                  .Select(x => x.v)
                                  .ToList();
        }

        public static List<long> GetLongs(this string input)
        {
                var r = new Regex(@"(-?\d+)");
                var matches = r.Matches(input);

                return matches.SelectMany(x => x.Captures)
                                  .Select(x => x.Value)
                                  .Select(x => { var p = long.TryParse(x, out var v); return(p, v);} )
                                  .Where(x => x.p)
                                  .Select(x => x.v)
                                  .ToList();
        }

        public static bool IsDigit(this char input)
        {
                return input.GetDigit().isDigit;
        }

        public static (bool isDigit, int val) GetDigit(this char input)
        {
                if(int.TryParse(new[] {input}, out var val))
                {
                        return (true, val);
                }
                return (false, -1);
        }
}

public class LambdaComparer<T> : IComparer<T>
{
        private Func<T?, T?, int> _comparison;
        public LambdaComparer(Func<T?, T?, int> comparison)
        {
                _comparison = comparison;
        }

        public int Compare(T? x, T? y)
        {
                return _comparison.Invoke(x, y);
        }
}


public class CRTCalculator
{
        private static BigInteger ModularInverse(BigInteger a, BigInteger b)
        {
                var b0 = b;
                BigInteger x0 = 0;
                BigInteger x1 = 1;

                if (b == 1)
                {
                        return 1;
                }

                while (a > 1)
                {
                        var q = a / b;

                        var temp = b;
                        b = a % b;
                        a = temp;

                        temp = x0;

                        x0 = x1 - q * x0;

                        x1 = temp;
                }

                // Make x1 positive  
                if (x1 < 0)
                {
                        x1 += b0;
                }

                return x1;
        }

        public static BigInteger CRT(IEnumerable<(int num, int rem)> values)
        {
                var prod = values.Aggregate(new BigInteger(1), (acc, x) => x.num * acc);

                BigInteger result = 0;

                foreach (var (num, rem) in values)
                {
                        var pp = prod / num;
                        result += rem * ModularInverse(pp, num) * pp;
                }

                var res = result % prod;
                
                return res;
        }

}