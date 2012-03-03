using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DuplicateFinder;
using System.IO;

namespace DuplicateFinder.Tests
{
    [TestFixture]
    public class DuplicateFileFinderTests
    {
        [Test]
        public void IsExistingPath_validPathLowerCased_ReturnTrue()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder();

            bool result = duplicateFileFinder.DoesPathExist(@"c:\temp");

            Assert.IsTrue(result);

        }

        [Test]
        public void CompareFiles_FilesAreEqual_ReturnTrue()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder();

            byte[] byteArray = GetByteArray(".");
            MemoryStream file1 = new MemoryStream(byteArray);
            MemoryStream file2 = new MemoryStream(byteArray);
            bool result = duplicateFileFinder.CompareStreams(file1, file2);

            Assert.IsTrue(result);
        }

        [Test]
        public void CompareStreams_FilesAreEqual_ReturnFalse()
        {
            DuplicateFileFinder duplicateFileFinder = new DuplicateFileFinder();

            byte[] byteArray1 = GetByteArray(".");
            byte[] byteArray2 = GetByteArray(",");
            MemoryStream file1 = new MemoryStream(byteArray1);
            MemoryStream file2 = new MemoryStream(byteArray2);
            bool result = duplicateFileFinder.CompareStreams(file1, file2);

            Assert.IsFalse(result);
        }



        private byte[] GetByteArray(string byteText)
        {
            return Encoding.ASCII.GetBytes(byteText);
        }
    }
}
