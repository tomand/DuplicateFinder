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
        string[] directory = new string[1] { "directory1" };
       
        [Test]
        public void CompareFiles_FilesAreEqual_ReturnTrue()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());

            bool result = duplicateFileFinder.CompareFiles(GetCheckFile("file"), GetCheckFile("file"));

            Assert.IsTrue(result);
        }

        [Test]
        public void CompareFiles_FilesAreNotEqualLength_ReturnFalse()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());

            bool result = duplicateFileFinder.CompareFiles(GetCheckFile("file1"), GetCheckFile("file22"));

            Assert.IsFalse(result);
        }


        [Test]
        public void CompareFiles_FilesAreNotEqual_ReturnFalse()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());

            bool result = duplicateFileFinder.CompareFiles(GetCheckFile("file1"), GetCheckFile("file2"));

            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFiles_FirstFileNotFound_ThrowsFileNotFoundException()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());
            duplicateFileFinder.CompareFiles(GetCheckFile("notfile"), GetCheckFile("file"));
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void CompareFiles_SecondFileNotFound_ThrowsFileNotFoundException()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(new FileSystemWrapperStub());
            duplicateFileFinder.CompareFiles(GetCheckFile("file"), GetCheckFile("notfile"));
        }

        private CheckFile GetCheckFile(string file)
        {
            return new CheckFile(file, new FileSystemWrapperStub());
        }

        [Test]
        public void FindDuplicateFilesInDirectory_OnlyOneFileInFolder_ResultContainsOneCheckedFile()
        {
            var mock = new Mock<IFileSystemWrapper>();
            
            string file1 = "file1";
           
            mock.Setup(fs => fs.GetDirectories(directory[0])).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory[0], "*")).Returns(new string[] { file1 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
                      
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory);
            Assert.IsTrue(result.First().Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_NoDuplicatesFound_FileIsMarkedAsChecked()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithNoDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();

            Assert.IsTrue(result.Checked);
        }


        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_FileIsMarkedAsChecked()
        {     
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();
            
            Assert.IsTrue(result.Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_NoDuplicatesFound_FileIsMarkedAsNoDuplicate()
        {
            string[] directory = new string[1] {"directory1"};

            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithNoDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();

            Assert.IsFalse(result.IsDuplicated);
           
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_FileIsMarkedAsDuplicate()
        {
            
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();
            Assert.IsTrue(result.IsDuplicated);            
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_DublicateFileIsMarkedAsChecked()
        {
            
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).Last();
            Assert.IsTrue(result.Checked);
        }

        [Test]
        public void FindDuplicateFilesInDirectory_DuplicatesFound_DublicateFileIsMarkedAsDuplicate()
        {
            
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(GetMockedFileSystemWrapperWithDuplicates(directory[0]));

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).Last();
            Assert.IsTrue(result.IsDuplicated);
            
        }

        [Test]
        public void Send_In_two_different_directories_and_both_are_searched()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory1 = "directory1";
            string directory2 = "directory2";
            string file1 = "file1";
            string file2 = "file2";           
            mock.Setup(fs => fs.GetDirectories(directory1)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory1, "*")).Returns(new string[] { file1 });
            mock.Setup(fs => fs.GetDirectories(directory2)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory2, "*")).Returns(new string[] { file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);
            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file1));
            DuplicateFileFinder dff = new DuplicateFileFinder(mock.Object);

            var result = dff.FindDuplicateFilesInDirectory(new string[] {directory1, directory2});
            Assert.IsTrue(result[0].IsDuplicated, string.Format("file0: {0}, file1:{1}", result[0].FilePath, result[1].FilePath));
            
        }

        [Test]
        public void Send_In_a_directory_and_subdirectory_and_subdirectory_is_only_in_once()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory1 = @"d:\directory1";
            string directory2 = @"d:\directory1\directory2"; ;
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory1)).Returns(new string[] {directory2});
            //mock.Setup(fs => fs.GetFiles(directory1, "*")).Returns(new string[] { file1 });
            mock.Setup(fs => fs.GetDirectories(directory2)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory2, "*")).Returns(new string[] { file1 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);
            FileSystemWrapperStub fsw = new FileSystemWrapperStub();

            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));
            DuplicateFileFinder dff = new DuplicateFileFinder(mock.Object);

            var result = dff.FindDuplicateFilesInDirectory(new string[] { directory1, directory2 });
            Assert.AreEqual(1, result.Count);

        }

        private IFileSystemWrapper GetMockedFileSystemWrapperWithNoDuplicates(string directory)
        {
            var mock = new Mock<IFileSystemWrapper>();
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file2));
            return mock.Object;
        }

        private IFileSystemWrapper GetMockedFileSystemWrapperWithDuplicates(string directory)
        {
            var mock = new Mock<IFileSystemWrapper>();
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[0]);
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(() => fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(() => fsw.GetFileStream(file1));
            return mock.Object;
        }


    }
}
