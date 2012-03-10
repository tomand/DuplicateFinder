using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DuplicateFinder
{
    public class DuplicateFileFinder
    {
        const int BYTES_TO_READ = sizeof(Int64);
        IFileSystemWrapper fileSystem;

        public DuplicateFileFinder()
        {
            fileSystem = new FileSystemWrapper();
        }

        public DuplicateFileFinder(IFileSystemWrapper fileSystemWrapper)
        {
            fileSystem = fileSystemWrapper;
        }

        public List<CheckFile> FindDuplicateFilesInDirectory(string directoryToSearch)
        {
            List<CheckFile> files = new List<CheckFile>();

            files = GetFiles(directoryToSearch, "*").Select(f => new CheckFile(f)).ToList<CheckFile>();
          
            int fileCounter = 1;
            foreach (CheckFile checkFile in files)
            {
                if (!checkFile.Checked)
                {
                    checkFile.FileId = fileCounter;
                    FindDuplicateFiles( checkFile, ref files);
                    fileCounter++;
                }
                
            }

            return files;
        }

       private List<string> GetFiles(string directoryPath, string searchPattern)
        {
            List<string> result = new List<string>();
            try
            {
                result.AddRange(fileSystem.GetFiles(directoryPath, searchPattern));

                foreach (string d in fileSystem.GetDirectories(directoryPath))
                {
                    if (d != directoryPath)
                        result.AddRange(GetFiles(d, searchPattern)); 
                }
            }
            catch (Exception)
            {
                ;
            }
            return result;
        }

        private void FindDuplicateFiles(CheckFile checkFile, ref List<CheckFile> files)
        {
           
            foreach (CheckFile file in files)
            {
                if (checkFile.FilePath != file.FilePath && CompareFiles(checkFile.FilePath, file.FilePath))
                {
                    file.Checked = true;
                    file.IsDuplicated = true;
                    
                    checkFile.IsDuplicated = true;
                    file.FileId = checkFile.FileId;
                }
            }
            checkFile.Checked = true;
        }

       
        public bool CompareFiles(string filePath1, string filePath2)
        { 
            CheckFile(filePath1);
            CheckFile(filePath2);

            bool result = CompareStreams(fileSystem.GetFileStream(filePath1), fileSystem.GetFileStream(filePath2));

            return result;
        }

        private void CheckFile(string filePath)
        {
            if (!fileSystem.Exists(filePath))
                throw new FileNotFoundException(string.Format("Cant't find the file: {0}", filePath));
        }

        private bool CompareStreams(Stream file1, Stream file2)
        {
            int iterations = (int)Math.Ceiling((double)file1.Length / BYTES_TO_READ);
            
            byte[] b1 = new byte[BYTES_TO_READ];
            byte[] b2 = new byte[BYTES_TO_READ];

            for (int i = 0; i < iterations; i++)
            {
                file1.Read(b1, 0, BYTES_TO_READ);
                file2.Read(b2, 0, BYTES_TO_READ);

                if (BitConverter.ToInt64(b1, 0) != BitConverter.ToInt64(b2, 0))
                    return false;
            }


            return true;
        }

        
    }
}
