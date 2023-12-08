using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Sources;
using Microsoft.VisualBasic;

public class WorkInProgress : IDay
{
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);
                var directions = lines.First();
                var nodes = new Dictionary<string, (string self, string l, string r)>();
                foreach(var line in lines)
                {
                        var stuff = line.Split('=');
                        var source = stuff[0].Trim();
                        var lr = stuff[1].Split(',');
                        var l = lr[0].Trim('(').Trim();
                        var r = lr[1].Trim(')').Trim();
                        nodes[source] = (source, l, r);
                }

                var done = false;
                var counter = 0;
                var curNode = nodes["AAA"];
                while(!done)
                {
                        foreach(var d in directions)
                        {
                                counter++;
                                if(d == 'L')
                                {
                                        curNode = nodes[curNode.l];
                                }
                                else
                                {
                                        curNode = nodes[curNode.r];
                                }
                                if(curNode.self == "ZZZ")
                                {
                                        done = true;
                                        break;
                                }
                        }
                }
                Console.WriteLine(counter);
        }

}
