using Microsoft.VisualBasic.ApplicationServices;
using WinLess.Models;

namespace WinLess
{
    public class SingleInstanceController : WindowsFormsApplicationBase
    {
        public SingleInstanceController()
        {
            this.IsSingleInstance = true;
            this.StartupNextInstance += this_StartupNextInstance;
        }

        private void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs eventArgs)
        {
            var args = new string[eventArgs.CommandLine.Count];
            eventArgs.CommandLine.CopyTo(args, 0);

            var commandLineArguments = new CommandArguments(args);

            if (commandLineArguments.ConsoleExit)
            {
                return;
            }

            var form = (MainForm)this.MainForm;

            if (commandLineArguments.HasArguments)
            {
                form.LoadDirectories(commandLineArguments);
            }
            else
            {
                form.RestoreFromTray();
            }
        }

        protected override void OnCreateMainForm()
        {
            this.MainForm = new MainForm();
        }
    }
}