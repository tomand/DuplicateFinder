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

        //- Den testade filen blir markerad som testad även om det inte finns några dubletter
        [Test]
        public void FindDuplicateFilesInDirectory_NoDuplicatesFound_FileIsMarkedAsChecked()
        {
            var mock = new Mock<IFileSystemWrapper>();
            string directory = "directory1";
            string file1 = "file1";
            string file2 = "file2";
            mock.Setup(fs => fs.GetDirectories(directory)).Returns(new string[] { directory });
            mock.Setup(fs => fs.GetFiles(directory, "*")).Returns(new string[] { file1, file2 });
            mock.Setup(fs => fs.Exists(file1)).Returns(true);
            mock.Setup(fs => fs.Exists(file2)).Returns(true);

            FileSystemWrapperStub fsw = new FileSystemWrapperStub();
            mock.Setup(fs => fs.GetFileStream(file1)).Returns(fsw.GetFileStream(file1));
            mock.Setup(fs => fs.GetFileStream(file2)).Returns(fsw.GetFileStream(file2));
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder(mock.Object);

            var result = duplicateFileFinder.FindDuplicateFilesInDirectory(directory).First();
            Assert.IsTrue(result.Checked);
        }

        
    }
}
