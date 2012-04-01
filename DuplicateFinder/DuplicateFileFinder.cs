using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DuplicateFinder
{
    public class DuplicateFileFinder
    {
        
        IFileSystemWrapper fileSystem;

        public delegate void CheckedFileEventHandler();
        public event CheckedFileEventHandler RaiseCheckedFileEvent;

        public delegate void StartReadingFilesEventHandler();
        public event StartReadingFilesEventHandler RaiseStartReadingFilesEvent;

        public delegate void EndReadingFilesEventHandler();
        public event EndReadingFilesEventHandler RaiseEndReadingFilesEvent;

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
            OnStartReadingFiles();

            files = GetFiles(directoryToSearch, "*").Select(f => new CheckFile(f, fileSystem)).ToList<CheckFile>();
            OnEndReadingFiles();
            int fileCounter = 1;
            foreach (CheckFile checkFile in files)
            {
                if (!checkFile.Checked)
                {
                    OnCheckedFile();
                    checkFile.FileId = fileCounter;
                    FindDuplicateFiles( checkFile, ref files);
                    fileCounter++;
                }
                
            }

            OnCheckedFile();
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
              
            foreach (CheckFile file in files.Where(f => !f.Checked))
            {
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
            return file1.IsEqualTo(file2);

            
        }

       

        

        protected virtual void OnCheckedFile()
        {
            CheckedFileEventHandler handler = RaiseCheckedFileEvent;

            RaiseEvent(handler);
        }

        protected virtual void OnStartReadingFiles()
        {
            StartReadingFilesEventHandler handler = RaiseStartReadingFilesEvent;

            RaiseEvent(handler);
        }

        protected virtual void OnEndReadingFiles()
        {
            EndReadingFilesEventHandler handler = RaiseEndReadingFilesEvent;

            RaiseEvent(handler);
        }

        private static void RaiseEvent(Delegate handler)
        {
            if (handler != null)
            {
                handler.DynamicInvoke();
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
