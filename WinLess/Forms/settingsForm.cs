using System;
using System.Windows.Forms;

namespace WinLess
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            this.InitializeComponent();
            this.LoadSettings();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.SaveSettings();

            this.Close();
        }

        private void LoadSettings()
        {
            this.startWithWindowsCheckBox.Checked = Program.Settings.StartWithWindows;
            this.startMinimizedCheckBox.Checked = Program.Settings.StartMinified;
            this.useGloballyInstalledLessCheckbox.Checked = Program.Settings.UseGloballyInstalledLess;
            this.defaultMinifyCheckBox.Checked = Program.Settings.DefaultMinify;
            this.compileOnSaveCheckBox.Checked = Program.Settings.CompileOnSave;
            this.showSuccessMessagesCheckbox.Checked = Program.Settings.ShowSuccessMessages;
        }

        private void SaveSettings()
        {
            Program.Settings.StartWithWindows = this.startWithWindowsCheckBox.Checked;
            Program.Settings.StartMinified = this.startMinimizedCheckBox.Checked;
            Program.Settings.UseGloballyInstalledLess = this.useGloballyInstalledLessCheckbox.Checked;
            Program.Settings.DefaultMinify = this.defaultMinifyCheckBox.Checked;
            Program.Settings.CompileOnSave = this.compileOnSaveCheckBox.Checked;
            Program.Settings.ShowSuccessMessages = this.showSuccessMessagesCheckbox.Checked;

            Program.Settings.SaveSettings();
        }
    }
}
