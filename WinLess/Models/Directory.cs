using System;
using System.Collections.Generic;
using System.IO;

namespace WinLess.Models
{
    public class Directory
    {
        #region Constructor

        private Directory()
        {
        }

        public Directory(string path)
        {
            this.FullPath = path;
            this.LoadFiles();
        }

        #endregion Constructor

        #region Properties

        private string fullPath;

        public string FullPath
        {
            get => this.fullPath;
            set
            {
                var directoryInfo = new DirectoryInfo(value);
                if (directoryInfo.Exists)
                {
                    this.fullPath = directoryInfo.FullName;
                }
            }
        }

        public List<File> Files { get; set; }

        #endregion Properties

        #region Public Methods

        public void LoadFiles()
        {
            this.Files = new List<File>();
            this.Refresh();
        }

        public void Refresh()
        {
            var removedFiles = new List<File>();
            foreach (File file in this.Files)
            {
                if (!System.IO.File.Exists(file.FullPath))
                {
                    removedFiles.Add(file);
                }
            }
            foreach (File file in removedFiles)
            {
                this.Files.Remove(file);
            }

            this.AddFiles(new DirectoryInfo(this.FullPath));
        }

        public bool ContainsFile(string fullPath)
        {
            var result = false;
            foreach (File file in this.Files)
            {
                if (string.Compare(file.FullPath, fullPath, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public override string ToString()
        {
            return this.FullPath;
        }

        #endregion Public Methods

        #region Private Methods

        private void AddFiles(DirectoryInfo directoryInfo)
        {
            try
            {
                FileInfo[] files = directoryInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    if ((string.Compare(file.Extension, ".less", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                        (string.Compare(file.Extension, ".css", StringComparison.InvariantCultureIgnoreCase) == 0) && file.Name.Contains(".less")
                       ) && !this.ContainsFile(file.FullName))
                    {
                        this.Files.Add(new File(this, file.FullName));
                    }
                }

                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo subDirectoryInfo in subDirectories)
                {
                    this.AddFiles(subDirectoryInfo);
                }
            }
            catch
            {
                // ignored
            }
        }

        #endregion Private Methods
    }
}
