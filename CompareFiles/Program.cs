using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuplicateFinder;
using System.IO;


namespace CompareFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You need to provide a directory to search.");
                return;
            }
            
            DuplicateFileFinder df = new DuplicateFileFinder();
            //foreach (string file in df.FindDuplicateFiles(@"d:\TestFileCompare\orginal.txt", @"d:\TestFileCompare"))
            //    Console.WriteLine(file);
            foreach (CheckFile file in df.FindDuplicateFilesInDirectory(args[0]).OrderBy(f => f.FileId).Where(f => f.IsDuplicated))
            {
                if (args.Length == 1)
                    Console.WriteLine(string.Format("{1} : {0}", file.FilePath, file.FileId));
                else if (args.Length == 2)
                    WriteFileToTextFile(string.Format("{1} : {0}", file.FilePath, file.FileId), args[1]);
            }
            Console.WriteLine("Please click enter...");
            Console.Read();
        }

        private static void WriteFileToTextFile(string fileText, string saveFilePath)
        {
            using (StreamWriter sw = File.AppendText(saveFilePath)) 
            {
                sw.WriteLine(fileText);
          
            }    
        }
       
        
    }
}
