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

        public bool DoesPathExist(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public List<CheckFile> FindDuplicateFilesInDirectory(string directoryToSearch)
        {
            List<CheckFile> files = Directory.GetFiles(directoryToSearch, "*", SearchOption.AllDirectories).Select(f => new CheckFile(f)).ToList<CheckFile>();

            int fileCounter = 1;
            foreach (CheckFile checkFile in files)
            {
                if (!checkFile.Checked)
                {
                    checkFile.FileId = fileCounter;
                    FindDuplicateFiles(checkFile, ref files);
                    fileCounter++;
                }
            }

            return files;
        }

        private void FindDuplicateFiles(CheckFile checkFile, ref List<CheckFile> files)
        {
            foreach (CheckFile file in files)
            {
                if (checkFile.FilePath != file.FilePath && CompareFiles(checkFile.FilePath, file.FilePath))
                {
                    file.Checked = true;
                    file.IsDuplicated = true;
                    checkFile.Checked = true;
                    checkFile.IsDuplicated = true;
                    file.FileId = checkFile.FileId;
                }
            }
        }

        public List<CheckFile> FindFilesDuplicateFiles(string filePath, string directoryToSearch)
        {
            CheckFile checkFile = new CheckFile(filePath);

            List<CheckFile> checkedFiles = new List<CheckFile>();

            var files = Directory.GetFiles(directoryToSearch, "*", SearchOption.AllDirectories);
            
            foreach (string file in files)
            {
                if (filePath != file && CompareFiles(filePath, file))
                {
                    CheckFile comparedFile = new CheckFile(file);
                    comparedFile.Checked = true;
                    comparedFile.IsDuplicated = true;
                    comparedFile.FileId = checkFile.FileId;
                    checkedFiles.Add(comparedFile);
                }
            }

            return checkedFiles;
        }

        public bool CompareFiles(string filePath1, string filePath2)
        { 
            CheckFile(filePath1);
            CheckFile(filePath2);

            bool result = CompareStreams(new FileStream(filePath1, FileMode.Open, FileAccess.Read), new FileStream(filePath2, FileMode.Open, FileAccess.Read));

            return result;
        }

        private void CheckFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("Cant't find the file: {0}", filePath));
        }

        public bool CompareStreams(Stream file1, Stream file2)
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
