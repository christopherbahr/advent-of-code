// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

var timedRun = true;
var iterCount = 1;

var ns = "AOC22";

try
{
        var timer = new Stopwatch();
        var wipArr = new WorkInProgress[iterCount];
        for(var i = 0; i < iterCount; i++)
        {
                wipArr[i] = new WorkInProgress();
        }
        timer.Start();
        for(var i = 0; i < iterCount; i++)
        {
                wipArr[i].Execute();
        }
        var elapsed = timer.ElapsedMilliseconds;
        Console.WriteLine($"{iterCount} run{(iterCount == 1 ? "" : "s")} of WIP Finished in (avg): {elapsed / iterCount}");
}
catch(Exception e)
{
        Console.WriteLine("FAILURE");
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
}

if(timedRun)
{
        using var writer = new StringWriter();
        var cOut = Console.Out;

        var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        // find all the days
        var dayClasses = types.Where(x => x.Namespace == ns).Where(x => x.GetInterfaces().Any(x => x == typeof(IDay))).Where(x => x.Name != "WorkInProgress").OrderBy(x => x.Name.GetInts().First());
        var zippedList = Enumerable.Range(1, dayClasses.Count()).Zip(dayClasses);
        List<IDay> days = new List<IDay>();
        foreach(var (i, d) in zippedList)
        {
                var nd = (IDay)d.GetConstructors().Single().Invoke(new object []{});
                var year = ns.GetInts().Single();
                var input = $"inputs/20{year}/{i}.in";
                nd.inputFile = input;
                days.Add(nd);
        }

        Console.WriteLine("=====  Starting Timed Run =====");
        var timer = new Stopwatch();
        timer.Start();
        var times = new List<long>();
        foreach(var (i, day) in days.Enumerate())
        {
                var ts = timer.ElapsedMilliseconds;
                Console.SetOut(writer);
                day.Execute();
                Console.SetOut(cOut);
                var te = timer.ElapsedMilliseconds;
                Console.WriteLine($"Day {i + 1}: {te - ts}");
                times.Add(te-ts);
        }
        Console.WriteLine($"Total Time: {times.Sum()}");
        Console.WriteLine("=====  End Timed Run =====");
}
