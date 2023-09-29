// Switching from naive nested loops to the sorted list lets this easily complete in under 5ms.
// Something that built on pointers into the sorted list could be even faster because you could leave
// the second pointer and know which direction to move it
namespace AOC20;
public class Day1 : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var nums = new SortedList<int, int>();
                foreach (var line in lines)
                {
                        var num = int.Parse(line);
                        nums.Add(num, num);
                }

                var (_, n1, n2) = FindTwoThatSum(nums, 2020);
                Console.WriteLine(n1 * n2);

                foreach (var (n, _) in nums)
                {
                        var (found, n21, n22) = FindTwoThatSum(nums, 2020 - n);
                        if(found)
                        {
                                Console.WriteLine(n * n21 * n22);
                                return;
                        }
                }
        }

        private (bool, int, int) FindTwoThatSum(SortedList<int, int> nums, int sum)
        {
                foreach(var (num, _) in nums)
                {
                        if(num > sum)
                        {
                                continue;
                        }
                        if(nums.ContainsKey(sum - num))
                        {
                                return (true, num, sum - num);
                        }
                }
                return (false, 0, 0);
        }

}