using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DuplicateFinder;
using System.IO;
using Moq;


namespace DuplicateFinder.Tests
{
    [TestFixture]
    public class DuplicateFileFinderTests
    {

       
        [Test]
        public void CompareFiles_FilesAreEqual_ReturnTrue()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());

            bool result = duplicateFileFinder.CompareFiles("file", "file");

            Assert.IsTrue(result);
        }

        [Test]
        public void CompareFiles_FilesAreNotEqual_ReturnFalse()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());

            bool result = duplicateFileFinder.CompareFiles("file1", "file2");

            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFiles_FirstFileNotFound_ThrowsFileNotFoundException()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());
            duplicateFileFinder.CompareFiles("notfile","file");
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFiles_SecondFileNotFound_ThrowsFileNotFoundException()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());
            duplicateFileFinder.CompareFiles("file", "notfile");
        }

        [Test]
        public void FindDuplicateFilesInDirectory_OnlyOneFileInFolder_ResultContainsOneCheckedFile()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
           
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
                      
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory);
            Assert.IsTrue(result.First().Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_NoDuplicatesFound_FileIsMarkedAsChecked()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));

            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();

            Assert.IsTrue(result.Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_FileIsMarkedAsChecked()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = file1;
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();
            
            Assert.IsTrue(result.Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_NoDuplicatesFound_FileIsMarkedAsNoDuplicate()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));

            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();

            Assert.IsFalse(result.IsDuplicated);
           
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_FileIsMarkedAsDuplicate()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);
            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file1));
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();
            Assert.IsTrue(result.IsDuplicated);            
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_DublicateFileIsMarkedAsChecked()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = file1;
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).Last();
            Assert.IsTrue(result.Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_DublicateFileIsMarkedAsDuplicate()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file1));
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).Last();
            Assert.IsTrue(result.IsDuplicated);
            
        }
        
    }
}
