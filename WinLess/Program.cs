using System;
using System.Windows.Forms;
using WinLessCore.Forms;
using WinLessCore.Helpers;
using WinLessCore.Models;

namespace WinLessCore
{
    internal static class Program
    {
        public static Settings Settings { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            var commandLineArguments = new CommandArguments(args);

            if (commandLineArguments.ConsoleExit)
            {
                return;
            }

            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            SingleInstance.Stop();
        }
    }
}
