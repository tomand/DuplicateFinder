using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuplicateFinder;
using System.IO;
using System.Threading;
using System.ComponentModel;


namespace CompareFiles
{
    class CompareFiles
    {

        static BackgroundWorker _bw;
        static bool writeResultToFile = false;
        static string resultFilePath = "";
        private static DuplicateFileFinder df;

        
        static void Main(string[] args)
        {
            df = new DuplicateFileFinder();
            df.RaiseCheckedFileEvent += HandleFileChecked;
            df.RaiseStartReadingFilesEvent += HandleStartReadingFiles;
            df.RaiseEndReadingFilesEvent += HandleEndReadingFiles;



            if (args.Length == 0)
            {
                Console.WriteLine("You need to provide a directory to search.");
                return;
            }

            writeResultToFile = args.Length > 1;
            if (writeResultToFile)
                resultFilePath = args[2];


            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };


            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            _bw.RunWorkerAsync(args[0]);


            //foreach (CheckFile file in df.FindDuplicateFilesInDirectory(args[0]).OrderBy(f => f.FileId).Where(f => f.IsDuplicated))
            //{
            //    if (args.Length == 1)
            //        Console.WriteLine(string.Format("{1} : {0}", file.FilePath, file.FileId));
            //    else if (args.Length == 2)
            //        WriteFileToTextFile(string.Format("{1} : {0}", file.FilePath, file.FileId), args[1]);
            //}
            Console.Read();
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] directories = new string[2];
            directories[0] = e.Argument.ToString();
            var result = df.FindDuplicateFilesInDirectory(directories);

            if (!writeResultToFile)
                Console.WriteLine();
            foreach (CheckFile file in result.OrderBy(f => f.FileId).Where(f => f.IsDuplicated))
            {
                if (!writeResultToFile)
                    Console.WriteLine(string.Format("{1} : {0}", file.FilePath, file.FileId));
                else if (writeResultToFile)
                    WriteFileToTextFile(string.Format("{1} : {0}", file.FilePath, file.FileId), resultFilePath);
            }
        }

        static void bw_RunWorkerCompleted(object sender,
                                     RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Console.WriteLine("You canceled!");
            else if (e.Error != null)
                Console.WriteLine("Worker exception: " + e.Error.ToString());
            else
            {
                Console.WriteLine("Please click enter...");

            }     // from DoWork
        }

        static void bw_ProgressChanged(object sender,
                                  ProgressChangedEventArgs e)
        {
            Console.WriteLine("Reached " + e.ProgressPercentage + "%");
        }


        private static void WriteFileToTextFile(string fileText, string saveFilePath)
        {
            using (StreamWriter sw = File.AppendText(saveFilePath))
            {
                sw.WriteLine(fileText);

            }
        }

       private static void HandleFileChecked()
        {
            Console.Write(string.Format("Files left: {0}            \r", df.FilesLeft, df.TotalFiles));
        }

        private static void HandleStartReadingFiles()
       {
           Console.WriteLine(string.Format("Please wait, reading files..."));
       }

        private static void HandleEndReadingFiles()
        {
            Console.WriteLine(string.Format("{0} files found. Starting to compare files.", df.TotalFiles));
            //Console.WriteLine("");
        }


    }
}
