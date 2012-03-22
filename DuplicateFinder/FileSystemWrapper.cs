using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DuplicateFinder
{
    public interface IFileSystemWrapper 
    {

        IEnumerable<string> GetFiles(string directoryPath, string searchPattern);

        IEnumerable<string> GetDirectories(string directoryPath);

        bool Exists(string filePath);

        Stream GetFileStream(string filePath);

         long GetFileSize(string filePath1);
    }

   public class FileSystemWrapper : IFileSystemWrapper
    {
       public IEnumerable<string> GetFiles(string directoryPath, string searchPattern)
        {
            return Directory.GetFiles(directoryPath, searchPattern);
        }

       public IEnumerable<string> GetDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

       public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

       public Stream GetFileStream(string filePath)
       {
           return new FileStream(filePath, FileMode.Open, FileAccess.Read);
       }

       public long GetFileSize(string filePath)
       {
           if (!File.Exists(filePath))
               return 0;
           return new FileInfo(filePath).Length;
       }
    }

    public class FileSystemWrapperStub : IFileSystemWrapper
    {
        public IEnumerable<string> GetFiles(string directoryPath, string searchPattern)
        {
            return new string[]{"file1", "file2"};
        }

        public IEnumerable<string> GetDirectories(string directoryPath)
        {
            return new string[]{"directory1", "directory2"};
        }

        public bool Exists(string filePath)
        {
            return (filePath.StartsWith("file"));
        }

        public Stream GetFileStream(string filePath)
       {
           return new MemoryStream(GetByteArray(filePath));
       }

        private byte[] GetByteArray(string byteText)
        {
            return Encoding.ASCII.GetBytes(byteText);
        }

        public long GetFileSize(string filePath)
        {
            return filePath.Length;
        }
    }
}
