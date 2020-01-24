using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NDesk.Options;

namespace WinLessCore.Models
{
    public class CommandArguments
    {
        public CommandArguments(string[] args)
        {
            this.ClearDirectories = false;
            this.ConsoleExit = false;
            this.HasArguments = args.Length > 0;
            this.InitialCompile = false;
            this.Minify = false;
            this.ShowHelp = false;

            if (!this.HasArguments)
            {
                return;
            }

            this.DirectoryPaths = new List<string>();

            var optionSet = new OptionSet
            {
                {
                    "d|directory=",
                    "The {DIRECTORY} you want WinLess to watch. Can be used multiple times. Directories are added to the current directory list.",
                    v=> this.DirectoryPaths.Add(v)
                },
                {
                    "minify",
                    "Add the 'minify' flag to have minification enabled.",
                    v=> this.Minify = v != null
                },
                {
                    "compile",
                    "Add the 'compile' flag to do an initial compile of the LESS files.",
                    v=> this.InitialCompile = v != null
                },
                {
                    "clear",
                    "Add the 'clear' flag to clear the current directory list",
                    v=> this.ClearDirectories = v != null
                },
                {
                    "h|help",
                    "Show this message and exit",
                    v=> this.ShowHelp = v != null
                }
            };

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException e)
            {
                //Make sure we have a Console
                if (!AttachConsole(-1))
                {
                    AllocConsole();
                }
                Console.WriteLine(@"An exception occured when try to parse the command line arguments.");
                Console.WriteLine(e.Message);
                Console.WriteLine(@"Try `WinLess.exe --help' for more information.");
                this.ConsoleExit = true;
            }

            if (!this.ShowHelp)
            {
                return;
            }

            //Make sure we have a Console
            if (!AttachConsole(-1))
            {
                AllocConsole();
            }

            Console.WriteLine(@"Winless can be used with command line arguments. This can be useful if you create 'startup' scripts for your projects. Note: WinLess is single instance. If WinLess is already running your arguments will be applied to the running instance. WinLess accepts the following arguments:");
            optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine(@"Example usage: WinLess.exe -d ""C:\projects\project1"" -d ""C:\projects\project2"" --minify --compile --clear");
            this.ConsoleExit = true;
        }

        public bool ClearDirectories { get; set; }
        public bool ConsoleExit { get; set; }
        public bool HasArguments { get; set; }
        public bool InitialCompile { get; set; }
        public bool Minify { get; set; }
        public bool ShowHelp { get; set; }
        public List<string> DirectoryPaths { get; set; }

        #region Console Dll imports

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        #endregion Console Dll imports
    }
}
