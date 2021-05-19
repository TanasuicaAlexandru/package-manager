using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public class PackagesGraph
    {
        public List<Package> Vertices;                                  // packages as nodes 
        public List<Package> SortedPackages;                            // list of all lexicographically sorted packages
        public Dictionary<string, int> IndexToPackageName;              // dictionary that maps a name of a package to an index
        public int GraphSize;                                           // number (amount) of packages
        public int[,] AdjacencyMatrix;                                  // adjacency matrix to help us with direct dependencies

        // constructor
        public PackagesGraph(List<string> packageList)
        {
            List<Package> Packages = new List<Package>();
            Dictionary<string, int> IndexToName = new Dictionary<string, int>();
            int index = 0;

            foreach( var packageName in packageList)
            {
                var package = new Package(packageName);
                Packages.Add(package);
                IndexToName.Add(packageName, index);
                index++;

            }
            Vertices = Packages;
            GraphSize = Packages.Count;
            AdjacencyMatrix = new int[GraphSize, GraphSize];
            IndexToPackageName = IndexToName;
            SortedPackages = Vertices.OrderBy(x => x.Name).ToList();
        }


        // displaying the adjacency matrix
        public void PrintAdjacencyMatrix()
        {
            for(int i = 0; i < GraphSize; i++)
            {
                for(int j = 0; j < GraphSize; j++)
                {
                    Console.Write(AdjacencyMatrix[i, j] + " ");

                }
                Console.WriteLine();
            }
        }


        // displaying the pairs of index - name
        public void PrintIndexToPackageName()
        {
            foreach(var element in IndexToPackageName)
            {
                Console.WriteLine("Key: {0}, Value: {1}", element.Key, element.Value);
            }
        }


        //updates the adjacency matrix using the key-value pairs of names and indexes assigned in the constructor
        public void AddEdgesInAdjacencyMatrix(List<string> lines)
        {
            foreach(var line in lines)
            {
                string[] packages = line.Split(' ');

                var namePackage1 = packages[0];
                var namePackage2 = packages[1];

                int indexPackage1 = IndexToPackageName[namePackage1];
                int indexPackage2 = IndexToPackageName[namePackage2];

                AdjacencyMatrix[indexPackage1, indexPackage2] = 1;
            }
        }


        // task 1

        // just printing out all the packages sorted lexicographically
        public void PrintPackagesInLexicographicalOrder()
        {
            string pathtask1 = @"C:\Users\Alex\Desktop\PackageManager\PackageManager\TextFiles\task1.out";

            using (StreamWriter writer = new StreamWriter(pathtask1))
            {
                foreach (var element in SortedPackages)
                {
                    Console.WriteLine(element.Name);
                    writer.WriteLine(element.Name);
                }
            }

        }


        // task 2

        // bfs traversal to get the dependencies of a package
        public List<Package> GetPackageDependenciesAsList(Package package)
        {
            var queueBfs = new Queue<int>();
            var packageDependencyList = new List<Package>();
            var packageIndex = IndexToPackageName[package.Name];

            bool[] visited = new bool[GraphSize];

            visited[packageIndex] = true;
            queueBfs.Enqueue(packageIndex);

            while (queueBfs.Count != 0)
            {
                var index = queueBfs.Dequeue();

                packageDependencyList.Add(Vertices[index]);

                for (int j = 0; j < GraphSize; j++)
                {
                    if(AdjacencyMatrix[index, j] == 1)
                    {
                        if(visited[j] == false)
                        {
                            visited[j] = true;
                            queueBfs.Enqueue(j);
                        }
                    }
                }
            }

            // we remove the first element in the list ( the starting package )
            packageDependencyList.RemoveAt(0);

            // we sort it lexicographically
            var orderedDependencyList = packageDependencyList.OrderBy(x => x.Name).ToList();

            return orderedDependencyList;
        }

        
        public void PrintPackageAndDependencies()
        {
            string pathtask2 = @"C:\Users\Alex\Desktop\PackageManager\PackageManager\TextFiles\task2.out";

            using (StreamWriter writer = new StreamWriter(pathtask2))
            {
                foreach (var element in SortedPackages)
                {
                    Console.Write(element.Name + " ");
                    writer.Write(element.Name + " ");

                    var dependenciesList = GetPackageDependenciesAsList(element);

                    foreach (var elem in dependenciesList)
                    {
                        Console.Write(elem.Name + " ");
                        writer.Write(elem.Name + " ");
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                }
            }
        }



        // task 3

        // tried the topological sort approach, since we have a directed acyclic graph, but ended up doing the dfs/bfs approach
        // this method returns an array of indexes, topologically sorted
        public int[]  GetTopologicalIndexList()
        {
            int[] EdgesIntoVertex = new int[GraphSize];
            var queueTopological = new Queue<int>();
            int[] TopologicalArray = new int[GraphSize];

            for (int i = 0; i < GraphSize; i++)
            {
                for(int j = 0; j < GraphSize; j++)
                {
                    if (AdjacencyMatrix[i, j] == 1)
                        EdgesIntoVertex[j]++;
                }
            }


            for(int i = 0; i < GraphSize; i++)
            {
                if(EdgesIntoVertex[i] == 0)
                {
                    queueTopological.Enqueue(i);
                }
            }

            int count = 0;
            while(queueTopological.Count != 0)
            {
                var index = queueTopological.Dequeue();
                TopologicalArray[count++] = index;

                for(int j = 0; j < GraphSize; j++)
                {
                    if(AdjacencyMatrix[index, j] == 1)
                    {
                        EdgesIntoVertex[j]--;
                        if(EdgesIntoVertex[j] == 0)
                        {
                            queueTopological.Enqueue(j);
                        }
                    }
                }
            }

            return TopologicalArray;
        }

        // for each package that is available in a computer, we update the available packages in a frequency array, from 0 to 1
        // for each package that is available in a computer, we update the needed packages in a frequency array, if there are dependencies from the available package

        public void PrintMissingDependencyPackages(List<string> computerLines)
        {
            string pathtask3 = @"C:\Users\Alex\Desktop\PackageManager\PackageManager\TextFiles\task3.out";

            using (StreamWriter writer = new StreamWriter(pathtask3))
            {
                foreach (var line in computerLines)
                {
                    string[] packages = line.Split(' ');

                    var missingPackages = new List<Package>();

                    var neededPackages = new int[GraphSize];
                    var availablePackages = new int[GraphSize];

                    // we get the indexes of the available and needed packages of the current line in computers.in
                    foreach (var package in packages)
                    {
                        var currentIndexPackage = IndexToPackageName[package];
                        var currentPackage = Vertices[currentIndexPackage];


                        availablePackages[currentIndexPackage] = 1;

                        var ToAddDependenciesList = GetPackageDependenciesAsList(currentPackage);

                        foreach (var dependency in ToAddDependenciesList)
                        {
                            var dependencyIndex = IndexToPackageName[dependency.Name];
                            neededPackages[dependencyIndex] = 1;
                        }
                    }

                    // if we need a package and dont have it, we add it to the list of missing packages
                    for (int i = 0; i < GraphSize; i++)
                    {
                        if (neededPackages[i] == 1 && availablePackages[i] == 0)
                        {
                            missingPackages.Add(Vertices[i]);
                        }
                    }


                    var sortedMissingPackages = missingPackages.OrderBy(x => x.Name);

                    // printing out the sorted missing packages
                    foreach (var element in sortedMissingPackages)
                    {
                        Console.Write(element.Name + " ");
                        writer.Write(element.Name + " ");
                    }
                    Console.WriteLine();
                    writer.WriteLine();
                    
                    

                }
            }
        }
    }
}
