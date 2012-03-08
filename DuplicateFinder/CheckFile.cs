using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicateFinder
{
    public class CheckFile
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public CheckFile(string filePath)
        {
            // TODO: Complete member initialization
            this.filePath = filePath;
            this.Checked = false;
            this.IsDuplicated = false;
            this.FileId = 0;
        }

        public int FileId { get; set; }

        public bool IsDuplicated { get; set; }

        public bool Checked { get; set; }
    }
}
