using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DuplicateFinder
{
    public class CheckFile
    {
        private string filePath;
        IFileSystemWrapper fileSystem;
        const int BYTES_TO_READ = sizeof(Int64);

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public CheckFile(string filePath, IFileSystemWrapper fileSystemWrapper = null )
        {
            if (fileSystemWrapper != null)
                this.fileSystem = fileSystemWrapper;
            else
                this.fileSystem = FileSystemWrapper.GetInstance();

            CheckFileExists(filePath);
            this.filePath = filePath;
            this.Checked = false;
            this.IsDuplicated = false;
            this.FileId = 0;
            this.Size = fileSystemWrapper.GetFileSize(filePath);
           
        }

        public int FileId { get; set; }

        public bool IsDuplicated { get; set; }

        public bool Checked { get; set; }

        public long Size { get; set; }

        public Stream Stream {get {return fileSystem.GetFileStream(filePath);}}

        public Boolean IsEqualTo(CheckFile file)
        {
            

            bool result = false;
            if (this.Size == file.Size)
                result = CompareStreams(this.Stream, file.Stream);

            return result;

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

         private void CheckFileExists(string filePath)
        {
            if (!fileSystem.Exists(filePath))
                throw new FileNotFoundException(string.Format("Cant't find the file: {0}", filePath));
        }
    }
}
