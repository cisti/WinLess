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
            FullPath = path;
            LoadFiles();
        }

        #endregion Constructor

        #region Properties

        private string fullPath;

        public string FullPath
        {
            get => fullPath;
            set
            {
                var directoryInfo = new DirectoryInfo(value);
                if (directoryInfo.Exists)
                {
                    fullPath = directoryInfo.FullName;
                }
            }
        }

        public List<File> Files
        {
            get;
            set;
        }

        #endregion Properties

        #region Public Methods

        public void LoadFiles()
        {
            Files = new List<File>();
            this.Refresh();
        }

        public void Refresh()
        {
            var removedFiles = new List<File>();
            foreach (File file in Files)
            {
                if (!System.IO.File.Exists(file.FullPath))
                {
                    removedFiles.Add(file);
                }
            }
            foreach (File file in removedFiles)
            {
                Files.Remove(file);
            }

            AddFiles(new DirectoryInfo(this.FullPath));
        }

        public bool ContainsFile(string fullPath)
        {
            var result = false;
            foreach (File file in Files)
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
            return FullPath;
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
                        Files.Add(new File(this, file.FullName));
                    }
                }

                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo subDirectoryInfo in subDirectories)
                {
                    AddFiles(subDirectoryInfo);
                }
            }
            catch { }
        }

        #endregion Private Methods
    }
}