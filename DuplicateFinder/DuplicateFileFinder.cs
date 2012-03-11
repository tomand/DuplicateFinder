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

        public delegate void EventHandler();
        public event EventHandler RaiseCustomEvent;
        private List<DuplicateFinder.CheckFile> files;

        public DuplicateFileFinder()
        {
            fileSystem = new FileSystemWrapper();
            files = new List<CheckFile>();
        }

        public DuplicateFileFinder(IFileSystemWrapper fileSystemWrapper)
        {
            fileSystem = fileSystemWrapper;
            files = new List<CheckFile>();
        }

        public List<CheckFile> FindDuplicateFilesInDirectory(string directoryToSearch)
        {
            

            files = GetFiles(directoryToSearch, "*").Select(f => new CheckFile(f)).ToList<CheckFile>();
          
            int fileCounter = 1;
            foreach (CheckFile checkFile in files)
            {
                OnRaiseCustomEvent();
                if (!checkFile.Checked)
                {
                    checkFile.FileId = fileCounter;
                    FindDuplicateFiles( checkFile, ref files);
                    fileCounter++;
                }
                
            }
            OnRaiseCustomEvent();
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
            if (checkFile.Size == 0)
                checkFile.Size = fileSystem.GetFileSize(checkFile.FilePath);            
            foreach (CheckFile file in files)
            {
                if (file.Size == 0)
                    file.Size = fileSystem.GetFileSize(file.FilePath); 

                if (checkFile.FilePath != file.FilePath && CompareFiles(checkFile, file))
                {
                    file.Checked = true;
                    file.IsDuplicated = true;
                    
                    checkFile.IsDuplicated = true;
                    file.FileId = checkFile.FileId;
                }
            }
            checkFile.Checked = true;
        }

       
        public bool CompareFiles(CheckFile file1, CheckFile file2)
        { 
            CheckFile(file1.FilePath);
            CheckFile(file2.FilePath);

            bool result = false;
            if (file1.Size == file2.Size)
                result = CompareStreams(fileSystem.GetFileStream(file1.FilePath), fileSystem.GetFileStream(file2.FilePath));

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

        protected virtual void OnRaiseCustomEvent()
        {
            EventHandler handler = RaiseCustomEvent;

            if (handler != null)
            {
                handler();
            }
        }

        public  string TotalFiles { get {
           return files.Count.ToString();
        } }

        public  string FilesLeft { get {
           return files.Count(f => f.Checked == false).ToString();
        } }
    }
}
