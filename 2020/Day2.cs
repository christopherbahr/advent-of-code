namespace AOC20;
public class Day2 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var c1 = 0;
                var c2 = 0;
                var lines = File.ReadAllLines(inputFile);
                foreach (var (i, line) in lines.Enumerate())
                {
                        var nums = line.Split(' ').First().Split('-').ToArray();
                        var n1 = int.Parse(nums[0]);
                        var n2 = int.Parse(nums[1]);
                        var letter = line.Split(' ')[1].TrimEnd(':').Single();
                        var password = line.Split(' ').Last();
                        var letterCount = password.Count(x => x == letter);
                        //Console.WriteLine((min, max, letterCount, letter, password));
                        if(letterCount >= n1 && letterCount <= n2)
                        {
                                c1++;
                        }
                        var p1Good = password[n1 - 1] == letter;
                        var p2Good = password[n2 - 1] == letter;
                        if((p1Good && !p2Good) || (!p1Good && p2Good))
                        {
                                c2++;
                        }
                }
                Console.WriteLine(c1);
                Console.WriteLine(c2);
        }

}