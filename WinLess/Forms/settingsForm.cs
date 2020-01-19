using System;
using System.Windows.Forms;

namespace WinLess
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            startWithWindowsCheckBox.Checked = Program.Settings.StartWithWindows;
            startMinimizedCheckBox.Checked = Program.Settings.StartMinified;
            useGloballyInstalledLessCheckbox.Checked = Program.Settings.UseGloballyInstalledLess;
            defaultMinifyCheckBox.Checked = Program.Settings.DefaultMinify;
            compileOnSaveCheckBox.Checked = Program.Settings.CompileOnSave;
            showSuccessMessagesCheckbox.Checked = Program.Settings.ShowSuccessMessages;
        }

        private void SaveSettings()
        {
            Program.Settings.StartWithWindows = startWithWindowsCheckBox.Checked;
            Program.Settings.StartMinified = startMinimizedCheckBox.Checked;
            Program.Settings.UseGloballyInstalledLess = useGloballyInstalledLessCheckbox.Checked;
            Program.Settings.DefaultMinify = defaultMinifyCheckBox.Checked;
            Program.Settings.CompileOnSave = compileOnSaveCheckBox.Checked;
            Program.Settings.ShowSuccessMessages = showSuccessMessagesCheckbox.Checked;
            Program.Settings.SaveSettings();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }
    }
}