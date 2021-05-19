using System;

namespace PackageManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize deps.in file
            FileProcessing ReadDeps = new FileProcessing(@"C:\Users\Alex\Desktop\PackageManager\PackageManager\TextFiles\deps.in");

            var getLines = ReadDeps.ReadLinesFromFile();
            var packages = ReadDeps.GetPackages(getLines);

            // initialize computers.in file
            FileProcessing ReadComputers = new FileProcessing(@"C:\Users\Alex\Desktop\PackageManager\PackageManager\TextFiles\computers.in");

            var getLinesComputer = ReadComputers.ReadLinesFromFile();

            // initialize graph
            PackagesGraph Graph = new PackagesGraph(packages);
            Graph.AddEdgesInAdjacencyMatrix(getLines);

            // task 1
            Graph.PrintPackagesInLexicographicalOrder();

            // task 2
            Graph.PrintPackageAndDependencies();

            // task 3
            Graph.PrintMissingDependencyPackages(getLinesComputer);
        }
    }
}
