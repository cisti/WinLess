using NDesk.Options;
using System;
using System.Collections.Generic;

namespace WinLess.Models
{
    public class CommandArguments
    {
        public CommandArguments(string[] args)
        {
            HasArguments = args.Length > 0;
            ConsoleExit = false;
            ShowHelp = false;
            Minify = false;
            InitialCompile = false;
            ClearDirectories = false;

            if (!HasArguments)
            {
                return;
            }

            DirectoryPaths = new List<string>();
            var optionSet = new OptionSet
            {
                {
                    "d|directory=",
                    "The {DIRECTORY} you want WinLess to watch. Can be used multiple times. Directories are added to the current directory list.",
                    v=> DirectoryPaths.Add(v)
                },
                {
                    "minify",
                    "Add the 'minify' flag to have minification enabled.",
                    v=> Minify = v != null
                },
                {
                    "compile",
                    "Add the 'compile' flag to do an initial compile of the LESS files.",
                    v=> InitialCompile = v != null
                },
                {
                    "clear",
                    "Add the 'clear' flag to clear the current directory list",
                    v=> ClearDirectories = v != null
                },
                {
                    "h|help",
                    "Show this message and exit",
                    v=> ShowHelp = v != null
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
                ConsoleExit = true;
            }

            if (!ShowHelp)
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
            ConsoleExit = true;
        }

        #region Properties

        public bool HasArguments { get; set; }
        public bool ConsoleExit { get; set; }
        public bool ShowHelp { get; set; }

        public List<string> DirectoryPaths { get; set; }
        public bool Minify { get; set; }
        public bool InitialCompile { get; set; }
        public bool ClearDirectories { get; set; }

        #endregion Properties

        #region Console Dll imports

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        #endregion Console Dll imports
    }
}