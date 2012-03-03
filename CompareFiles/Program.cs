using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuplicateFinder;


namespace CompareFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            DuplicateFileFinder df = new DuplicateFileFinder();
            foreach (string file in df.FindDuplicateFiles(@"d:\TestFileCompare\orginal.txt", @"d:\TestFileCompare"))
                Console.WriteLine(file);

            Console.ReadLine();
        }
        
    }
}
