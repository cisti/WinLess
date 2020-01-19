using System;
using System.Windows.Forms;
using WinLess.Models;

namespace WinLess
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var controller = new SingleInstanceController();
            controller.Run(args);
        }
    }
}