using System.Numerics;

public class WorkInProgress : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var passports = new List<Dictionary<string, string>>();
                var passport = new Dictionary<string, string>();
                foreach (var (i, line) in lines.Enumerate())
                {
                        if(line == string.Empty)
                        {
                                passports.Add(passport);
                                passport = new Dictionary<string, string>();
                                continue;
                        }
                        var fields = line.Split(' ');
                        foreach(var field in fields)
                        {
                                var data = field.Split(':');
                                passport[data[0]] = data[1];
                        }
                }
                passports.Add(passport);

                var requiredKeys = new [] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
                var validCount = 0;
                var p2ValidCount = 0;

                foreach(var pp in passports)
                {
                        var p2Valid = true;
                        var p1Valid = true;
                        foreach(var k in requiredKeys)
                        {
                                if(!pp.ContainsKey(k))
                                {
                                        p1Valid &= false;
                                        p2Valid &= false;
                                        break;
                                }
                                p2Valid &= ValidateKey(k, pp[k]);
                        }
                        if(p1Valid)
                        {
                                validCount++;
                        }
                        if(p2Valid)
                        {
                                p2ValidCount++;
                        }
                }
                Console.WriteLine(validCount);
                Console.WriteLine(p2ValidCount);
        }

        private bool ValidateKey(string key, string val)
        {
                switch(key)
                {
                        case "byr":
                        {
                                var s = int.TryParse(val, out var n);
                                return s && n >= 1920 && n <= 2002;
                        }
                        case "iyr":
                        {
                                var s = int.TryParse(val, out var n);
                                return s && n >= 2010 && n <= 2020;
                        }
                        case "eyr":
                        {
                                var s = int.TryParse(val, out var n);
                                return s && n >= 2020 && n <= 2030;
                        }
                        case "hgt":
                        {
                                var inches = val.Contains("in");
                                var cm = val.Contains("cm");
                                if(inches)
                                {
                                        var n = val.GetInts().FirstOrDefault();
                                        return n >= 59 && n <= 76;
                                }
                                else if(cm)
                                {
                                        var n = val.GetInts().FirstOrDefault();
                                        return n >= 150 && n <= 193;
                                }
                                else
                                {
                                        return false;
                                }
                        }
                        case "hcl":
                        {
                                var firstChar = val.StartsWith('#');
                                var charCount = val.Length;
                                var valid = true;
                                foreach(var c in val.Skip(1))
                                {
                                        if((c >= 'a' || c <= 'f') || (c >= '0' && c <= '9'))
                                        {
                                                valid = true;
                                        }
                                        else
                                        {
                                                valid = false;
                                                break;
                                        }
                                }
                                return firstChar && valid && charCount == 7;
                        }
                        case "ecl":
                        {
                                var possible = new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
                                return possible.Contains(val);
                        }
                        case "pid":
                        {
                                var numbers = val.All(x => x >= '0' && x <= '9');
                                return numbers && val.Length == 9;
                        }
                }
                return false;

        }
}