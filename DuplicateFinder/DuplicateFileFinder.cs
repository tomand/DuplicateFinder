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

        public List<string> FindDuplicateFiles(string filePath, string directoryToSearch)
        {
            List<string> similarFiles = new List<string>();

            var files = Directory.GetFiles(directoryToSearch, "*.*", SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new Exception("Could not find any files");

            foreach (string file in files)
            {
                if (filePath != file && CompareFiles(filePath, file))
                    similarFiles.Add(file);
            }

            return similarFiles;
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
                throw new Exception("One or both of the files does not exists");
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
