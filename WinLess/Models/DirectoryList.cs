using System;
using System.Collections.Generic;
using System.IO;
using WinLessCore.Helpers;

namespace WinLessCore.Models
{
    public class DirectoryList
    {
        private List<FileSystemWatcher> fileSystemWatchers;
        private string previousFileChangedPath;
        private DateTime previousFileChangedTime = DateTime.Now;

        public List<Directory> Directories { get; set; }

        public DirectoryList()
        {
            this.Directories = new List<Directory>();
        }

        public void Initialize()
        {
            this.RemoveDeletedDirectories();
            this.CheckAllFilesForImports();
            this.StartWatchers();
        }

        public Directory AddDirectory(string path)
        {
            Directory directory = this.GetDirectory(path);

            if (directory != null)
            {
                return directory;
            }

            directory = new Directory(path);
            this.Directories.Add(directory);
            this.AddWatcher(directory);
            Program.Settings.SaveSettings();
            this.CheckAllFilesForImports();

            return directory;
        }

        public void RemoveDirectory(Directory directory)
        {
            this.Directories.Remove(directory);
            this.RemoveWatcher(directory.FullPath);
            Program.Settings.SaveSettings();
            this.CheckAllFilesForImports();
        }

        public void RemoveDeletedDirectories()
        {
            var removedDirectories = new List<Directory>();
            foreach (Directory directory in this.Directories)
            {
                if (!System.IO.Directory.Exists(directory.FullPath))
                {
                    removedDirectories.Add(directory);
                }
            }
            foreach (Directory directory in removedDirectories)
            {
                this.RemoveDirectory(directory);
            }
        }

        public void ClearDirectories()
        {
            var allDirectories = new List<Directory>(this.Directories);

            foreach (Directory directory in allDirectories)
            {
                this.Directories.Remove(directory);
                this.RemoveWatcher(directory.FullPath);
            }

            Program.Settings.SaveSettings();

            this.CheckAllFilesForImports();
        }

        public Directory GetDirectory(string path)
        {
            foreach (Directory directory in this.Directories)
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
            foreach (Directory directory in this.Directories)
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
            foreach (Directory directory in this.Directories)
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
                    if (this.fileSystemWatchers == null)
                    {
                        this.fileSystemWatchers = new List<FileSystemWatcher>();
                    }

                    var fileSystemWatcher = new FileSystemWatcher
                    {
                        Path = directory.FullPath,
                        IncludeSubdirectories = true,
                        Filter = "*.*",
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName |
                                       NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.Attributes
                    };
                    fileSystemWatcher.Changed += this.FileSystemWatcher_Changed;
                    fileSystemWatcher.EnableRaisingEvents = true;

                    this.fileSystemWatchers.Add(fileSystemWatcher);
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
                if (this.fileSystemWatchers == null)
                {
                    return;
                }

                foreach (FileSystemWatcher fileSystemWatcher in this.fileSystemWatchers)
                {
                    if (fileSystemWatcher.Path == path)
                    {
                        fileSystemWatcher.EnableRaisingEvents = false;
                        this.fileSystemWatchers.Remove(fileSystemWatcher);
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
            if (Program.Settings.CompileOnSave && this.IsNewFileChange(e) && this.IsLessFile(e.FullPath))
            {
                File file = Program.Settings.DirectoryList.GetFile(e.FullPath);

                if (file == null)
                {
                    return;
                }

                file.Compile();
                this.previousFileChangedPath = file.FullPath;
                this.previousFileChangedTime = DateTime.Now;
            }
        }

        private bool IsNewFileChange(FileSystemEventArgs e)
        {
            return this.previousFileChangedPath != e.FullPath || DateTime.Now - this.previousFileChangedTime > new TimeSpan(0, 0, 1);
        }

        private bool IsLessFile(string path)
        {
            return path.EndsWith(".less") || path.EndsWith(".less.css");
        }
    }
}
