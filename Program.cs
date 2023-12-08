// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;


var timedRun = false;
var iterCount = 1;
var cookieStr = string.Empty;
HttpClient? client = null;
var tryFetchInput = false;

foreach(var (i, arg) in args.Enumerate()) 
{
        if(arg == "-t" || arg == "--timed")
        {
                timedRun = true;
        }
        if(arg == "-c" || arg == "--cookie")
        {
                cookieStr = args[i + 1];
        }
        if(arg == "-i" || arg == "--iterations")
        {
                iterCount = int.Parse(args[i+1]);
        }
}

if(cookieStr != string.Empty)
{
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler() {CookieContainer = cookieContainer};
        client = new HttpClient(handler);
        var cookie = new Cookie("session", cookieStr);
        cookie.Domain = "adventofcode.com";
        cookieContainer.Add(cookie);
        tryFetchInput = true;
}

var namespaces = new [] {"AOC20", "AOC21", "AOC22", "AOC23"};

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
        Console.WriteLine("=====  Starting Timed Run =====");
        var timer = new Stopwatch();
        timer.Start();
        foreach(var ns in namespaces)
        {
                var year = ns.GetInts().Single();
                Console.WriteLine($"=====  {year} =====");
                // find all the days
                var dayClasses = types.Where(x => x.Namespace == ns).Where(x => x.GetInterfaces().Any(x => x == typeof(IDay))).Where(x => x.Name != "WorkInProgress").OrderBy(x => x.Name.GetInts().First());
                var zippedList = Enumerable.Range(1, dayClasses.Count()).Zip(dayClasses);
                List<IDay> days = new List<IDay>();
                foreach(var (i, d) in zippedList)
                {
                        var nd = (IDay)d.GetConstructors().Single().Invoke(new object []{});
                        var input = $"inputs/20{year}/{i}.in";
                        nd.inputFile = input;
                        if(!File.Exists(input) && tryFetchInput)
                        {
                                var resp = await client!.GetAsync($"https://adventofcode.com/20{year}/day/{i}/input");
                                if(resp.IsSuccessStatusCode)
                                {
                                        var data = await resp.Content.ReadAsStringAsync();
                                        data = data.TrimEnd('\n');
                                        //var splitData = data.Split(Environment.NewLine);
                                        File.WriteAllLines(input, new[] {data} );
                                }
                                else
                                {
                                        Console.WriteLine($"Missing input for 20{year} {i}. Expect failure.");
                                }

                        }
                        days.Add(nd);
                }

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
                Console.WriteLine($"Total Time {year}: {times.Sum()}");
        }
        Console.WriteLine("=====  End Timed Run =====");
        /*
        This gets nice output from 2022 which originally returnd (string, string) pairs
        and now just prints them.

        2021 is still chaos. Faster to implement and less boilerplate than 2022 was originally
        but just prints stuff randomly. 

        That's better for competitive and quick development but weirder for the optimizations at the end.
        var output = writer.ToString();
        var lines = output.Split(Environment.NewLine);
        foreach(var line in lines.Where(x => x.StartsWith('(')))
        {
                Console.WriteLine(line);
        }
        */
}
