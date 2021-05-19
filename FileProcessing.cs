using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public class FileProcessing
    {
        public string depsFilePath;

        public FileProcessing(string filePath)
        {
            depsFilePath = filePath;
        }

        public List<string> ReadLinesFromFile()
        {
            List<string> Lines = new List<string>();

            var lines = File.ReadLines(depsFilePath);
            foreach(var line in lines)
            {
                //Console.WriteLine(line);
                Lines.Add(line);
            }

            return Lines;
        }

        public List<string> GetPackages(List<string> lines)
        {
            List<string> PackagesList = new List<string>();
            foreach (var line in lines)
            {
                string[] packages = line.Split(' ');

                foreach(var package in packages)
                {
                    if (!PackagesList.Contains(package))
                    {
                        //Console.WriteLine(package);
                        PackagesList.Add(package);
                    }
                }
            }
            return PackagesList;
        }
    }
}
