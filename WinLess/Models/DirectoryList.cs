using System;
using System.Collections.Generic;
using System.IO;
using WinLess.Helpers;

namespace WinLess.Models
{
    public class DirectoryList
    {
        private List<FileSystemWatcher> fileSystemWatchers;
        private string previousFileChangedPath = null;
        private DateTime previousFileChangedTime = DateTime.Now;

        public List<Directory> Directories { get; set; }

        public DirectoryList()
        {
            Directories = new List<Directory>();
        }

        public void Initialize()
        {
            RemoveDeletedDirectories();
            CheckAllFilesForImports();
            StartWatchers();
        }

        public Directory AddDirectory(string path)
        {
            Directory directory = GetDirectory(path);
            if (directory == null)
            {
                directory = new Directory(path);
                Directories.Add(directory);
                AddWatcher(directory);
                Program.Settings.SaveSettings();
                CheckAllFilesForImports();
            }

            return directory;
        }

        public void RemoveDirectory(Directory directory)
        {
            Directories.Remove(directory);
            RemoveWatcher(directory.FullPath);
            Program.Settings.SaveSettings();
            CheckAllFilesForImports();
        }

        public void RemoveDeletedDirectories()
        {
            var removedDirectories = new List<Directory>();
            foreach (Directory directory in Directories)
            {
                if (!System.IO.Directory.Exists(directory.FullPath))
                {
                    removedDirectories.Add(directory);
                }
            }
            foreach (Directory directory in removedDirectories)
            {
                RemoveDirectory(directory);
            }
        }

        public void ClearDirectories()
        {
            var allDirectories = new List<Directory>(Directories);

            foreach (Directory directory in allDirectories)
            {
                Directories.Remove(directory);
                RemoveWatcher(directory.FullPath);
            }

            Program.Settings.SaveSettings();

            CheckAllFilesForImports();
        }

        public Directory GetDirectory(string path)
        {
            foreach (Directory directory in Directories)
            {
                if (path == directory.FullPath || (path.StartsWith(directory.FullPath)))
                {
                    return directory;
                }
            }

            return null;
        }

        public File GetFile(string path)
        {
            foreach (Directory directory in Directories)
            {
                File file = directory.Files.Find(f => string.Compare(f.FullPath, path, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (file != null)
                {
                    return file;
                }
            }
            return null;
        }

        private void CheckAllFilesForImports()
        {
            foreach (Directory directory in this.Directories)
            {
                foreach (File file in directory.Files)
                {
                    file.ParentFiles = new List<File>();
                }
            }
            foreach (Directory directory in this.Directories)
            {
                foreach (File file in directory.Files)
                {
                    file.CheckForImports();
                }
            }
        }

        public void StartWatchers()
        {
            foreach (Directory directory in Directories)
            {
                this.AddWatcher(directory);
            }
        }

        private void AddWatcher(Directory directory)
        {
            try
            {
                if (System.IO.Directory.Exists(directory.FullPath))
                {
                    if (fileSystemWatchers == null)
                    {
                        fileSystemWatchers = new List<FileSystemWatcher>();
                    }

                    var fileSystemWatcher = new FileSystemWatcher
                    {
                        Path = directory.FullPath,
                        IncludeSubdirectories = true,
                        Filter = "*.*",
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName |
                                       NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.Attributes
                    };
                    fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                    fileSystemWatcher.EnableRaisingEvents = true;

                    fileSystemWatchers.Add(fileSystemWatcher);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
        }

        private void RemoveWatcher(string path)
        {
            try
            {
                if (fileSystemWatchers == null)
                {
                    return;
                }

                foreach (FileSystemWatcher fileSystemWatcher in fileSystemWatchers)
                {
                    if (fileSystemWatcher.Path == path)
                    {
                        fileSystemWatcher.EnableRaisingEvents = false;
                        fileSystemWatchers.Remove(fileSystemWatcher);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Program.Settings.CompileOnSave && IsNewFileChange(e) && IsLessFile(e.FullPath))
            {
                File file = Program.Settings.DirectoryList.GetFile(e.FullPath);

                if (file != null)
                {
                    file.Compile();
                    previousFileChangedPath = file.FullPath;
                    previousFileChangedTime = DateTime.Now;
                }
            }
        }

        private bool IsNewFileChange(FileSystemEventArgs e)
        {
            return previousFileChangedPath != e.FullPath || DateTime.Now - previousFileChangedTime > new TimeSpan(0, 0, 1);
        }

        private bool IsLessFile(string path)
        {
            return path.EndsWith(".less") || path.EndsWith(".less.css");
        }
    }
}