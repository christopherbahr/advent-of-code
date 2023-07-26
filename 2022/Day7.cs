namespace AOC22;
public class Day7 : IDay
{
        private class DirectoryTree
        {
                public string Name {get; set;}
                public Dictionary<string, int> Files {get; set;} = new Dictionary<string, int>();
                public Dictionary<string, DirectoryTree> ChildFolders = new Dictionary<string, DirectoryTree>();
                public DirectoryTree Parent {get; set;}
                public int Size {get; set;}
        }
        public string inputFile { get; set; } = "wip.in";
        public void Execute()
        {
                var lines = File.ReadAllLines(inputFile);

                var currentPath = new List<string>();
                var root = new DirectoryTree();
                var current = root;

                // need to use a regular for loop so we can consume lines inside the loop and iterate the loop
                // I'd rather consume one line at a time but I honestly don't remember how
                for(int i = 0; i < lines.Length; i++)
                {
                        var line = lines[i];
                        if (line.StartsWith('$'))
                        {
                                // this is a command
                                var parts = line.Split(' ');
                                // Populate the current tree and 
                                if (parts[1] == "ls")
                                {
                                        for(i++; i < lines.Length; i++)
                                        {
                                                var dirInfo = lines[i].Split(' ');
                                                var found = false;
                                                switch(dirInfo[0], dirInfo[1])
                                                {
                                                        case ("$", _):
                                                                i--;
                                                                break;
                                                        case ("dir", var dirName):
                                                                found = true;
                                                                current.ChildFolders[dirName] = new DirectoryTree{ Name = dirName, Parent = current };
                                                                break;
                                                        case (var fileSize, var fileName):
                                                                found = true;
                                                                current.Files[fileName] = int.Parse(fileSize);
                                                                break;
                                                }
                                                if(!found)
                                                {
                                                        break;
                                                }
                                        }

                                }
                                // switching directories
                                else if (parts[1] == "cd")
                                {
                                        // just moving up a directory
                                        if(parts[2] == "..")
                                        {
                                                current = current.Parent;
                                        }
                                        // back to root
                                        else if(parts[2] == "/")
                                        {
                                                current = root;
                                        }
                                        else
                                        {
                                                // navigate down a directory, this should already be populated
                                                current = current.ChildFolders[parts[2]];
                                        }
                                }
                                else
                                {
                                        Console.WriteLine("Unexpected command");
                                }
                        }
                        else
                        {
                                Console.WriteLine("Unexpected output");
                        }

                }

                PrettyPrint(root, 0);

                PopulateDirectorySize(root);

                var totalSize = 0;
                foreach(var dir in CandidateDirectories)
                {
                        totalSize += dir.Size;
                }

                // now onto part 2

                var neededSpace = 30000000;
                var consumedSpace = root.Size;

                var spaceLeft = 70000000 - consumedSpace;

                var toDelete = neededSpace - spaceLeft;

                Console.WriteLine($"Consumed: {consumedSpace} | Remaining: {spaceLeft} | Required {neededSpace} | Amount To Delete: {toDelete}");

                var smallestSoFar = int.MaxValue;

                foreach(var dir in FlatDirectories)
                {
                        if(dir.Size > toDelete && dir.Size < smallestSoFar)
                        {
                                smallestSoFar = dir.Size;
                        }
                }



                Console.WriteLine( (totalSize.ToString(), smallestSoFar.ToString()));
        }

        private void PrettyPrint(DirectoryTree tree, int depth)
        {
                Console.WriteLine($"{string.Concat(Enumerable.Repeat(" ", depth*2))} Folder: {tree.Name}");
                depth++;
                foreach(var file in tree.Files)
                {
                        Console.WriteLine(string.Concat(Enumerable.Repeat(" ", depth*2)) + "- " + file.Key + " " + file.Value);
                }
                foreach(var dir in tree.ChildFolders)
                {
                        PrettyPrint(dir.Value, depth);
                }
        }

        private List<DirectoryTree> CandidateDirectories = new List<DirectoryTree>();
        private List<DirectoryTree> FlatDirectories = new List<DirectoryTree>();

        private int PopulateDirectorySize(DirectoryTree tree)
        {
                var size = 0;
                foreach(var file in tree.Files)
                {
                        size += file.Value;
                }

                foreach(var folder in tree.ChildFolders)
                {
                        size += PopulateDirectorySize(folder.Value);
                }

                tree.Size = size;

                FlatDirectories.Add(tree);

                if(size < 100000)
                {
                        CandidateDirectories.Add(tree);
                }

                return size;
        }

        
}