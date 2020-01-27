using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinLessCore.Helpers;
using WinLessCore.Models;
using Directory = WinLessCore.Models.Directory;
using File = WinLessCore.Models.File;

namespace WinLessCore.Forms
{
    public partial class MainForm : Form
    {
        public static MainForm ActiveOrInActiveMainForm { get; set; }

        private delegate void AddCompileResultDelegate(CompileCommandResult result);

        private bool finishedLoading;

        private readonly FolderBrowserDialog folderBrowserDialog;
        private readonly SaveFileDialog saveFileDialog;

        public MainForm()
        {
            try
            {
                this.finishedLoading = false;
                ActiveOrInActiveMainForm = this;

                Program.Settings = Settings.LoadSettings();
                Program.Settings.DirectoryList.Initialize();

                this.InitializeComponent();
                this.InitFilesDataGridViewCheckAllCheckBox();

                this.foldersListBox.DataSource = Program.Settings.DirectoryList.Directories;
                this.compileResultsDataGridView.DataSource = new List<CompileCommandResult>();

                this.folderBrowserDialog = new FolderBrowserDialog();

                this.saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    Filter = @"CSS Files (.css)|*.css"
                };
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                WinApi.ShowToFront(this.Handle);
            }

            base.WndProc(ref message);
        }

        #region Event Handlers

        private void mainForm_Activated(object sender, EventArgs e)
        {
            if (this.finishedLoading)
            {
                return;
            }

            this.finishedLoading = true;

            if (Program.Settings.StartMinified)
            {
                this.MinimizeToTray();
            }
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            var commandLineArgs = new CommandArguments(args);
            if (commandLineArgs.HasArguments)
            {
                this.LoadDirectories(commandLineArgs);
            }
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Settings.SaveSettings();
        }

        private void foldersListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // Okay
            }
            else
            {
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            }
        }

        private void foldersListBox_DragDrop(object sender, DragEventArgs e)
        {
            var fullPaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fullPath in fullPaths)
            {
                var directoryInfo = new DirectoryInfo(fullPath);
                if (directoryInfo.Exists && !this.foldersListBox.Items.Contains(directoryInfo.FullName))
                {
                    Program.Settings.DirectoryList.AddDirectory(directoryInfo.FullName);
                    this.foldersListBox_DataChanged();
                    this.SelectDirectory();
                    Program.Settings.SaveSettings();
                }
            }
        }

        private void foldersListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.RemoveDirectory();
            }
        }

        private void foldersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.SelectDirectory();
        }

        private void foldersListBox_DataChanged()
        {
            ((CurrencyManager)this.foldersListBox.BindingContext[this.foldersListBox.DataSource]).Refresh();
            this.filesDataGridView_DataChanged();
        }

        private void filesDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Program.Settings.SaveSettings();
        }

        private void filesDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.filesDataGridView_OpenSelectedFile();
        }

        private void filesDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                this.filesDataGridView.CurrentCell = this.filesDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void InitFilesDataGridViewCheckAllCheckBox()
        {
            // add checkbox header
            Rectangle rect = this.filesDataGridView.GetCellDisplayRectangle(0, -1, true);
            // set checkbox header to center of header cell. +1 pixel to position correctly.
            rect.X = 10;
            rect.Y = 4;

            var checkAllFilesCheckbox = new CheckBox
            {
                Name = "checkboxHeader",
                Size = new Size(15, 15),
                Location = rect.Location
            };

            checkAllFilesCheckbox.CheckedChanged += this.checkAllFilesCheckbox_CheckedChanged;

            this.filesDataGridView.Controls.Add(checkAllFilesCheckbox);
        }

        private void checkAllFilesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.filesDataGridView.RowCount; i++)
            {
                this.filesDataGridView[0, i].Value = ((CheckBox)this.filesDataGridView.Controls.Find("checkboxHeader", true)[0]).Checked;
            }

            this.filesDataGridView.EndEdit();
        }

        private void filesDataGridView_DataChanged()
        {
            var files = (List<File>)this.filesDataGridView.DataSource;
            files.Sort((x, y) => string.CompareOrdinal(x.FullPath, y.FullPath));
            ((CurrencyManager)this.filesDataGridView.BindingContext[this.filesDataGridView.DataSource]).Refresh();
        }

        private void filesDataGridView_OpenSelectedFile()
        {
            DataGridViewCell cell = this.filesDataGridView.SelectedCells[0];
            var file = (File)cell.OwningRow.DataBoundItem;
            string filePath;
            if (cell.ColumnIndex == 1)
            {
                filePath = file.FullPath;
            }
            else
            {
                filePath = file.OutputPath;
            }

            if (System.IO.File.Exists(filePath))
            {
                var process = new Process { StartInfo = { FileName = filePath } };
                process.Start();
            }
        }

        private void openFiletoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.filesDataGridView_OpenSelectedFile();
        }

        private void fileOpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = this.filesDataGridView.SelectedCells[0];
            var file = (File)cell.OwningRow.DataBoundItem;
            string filePath;
            if (cell.ColumnIndex == 1)
            {
                filePath = file.FullPath;
            }
            else
            {
                filePath = file.OutputPath;
            }

            var fileInfo = new FileInfo(filePath);
            string directoryPath = fileInfo.DirectoryName;

            if (directoryPath != null && System.IO.Directory.Exists(directoryPath))
            {
                var process = new Process
                {
                    StartInfo = { FileName = directoryPath }
                };
                process.Start();
            }
        }

        private void fileSelectOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = this.filesDataGridView.SelectedCells[0];
            var file = (File)cell.OwningRow.DataBoundItem;
            var fileInfo = new FileInfo(file.OutputPath);

            this.saveFileDialog.InitialDirectory = fileInfo.DirectoryName;
            this.saveFileDialog.FileName = fileInfo.Name;

            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                file.OutputPath = this.saveFileDialog.FileName;
                this.filesDataGridView_DataChanged();
                Program.Settings.SaveSettings();
            }
        }

        private void addDirectoryButton_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string pathToAdd = this.folderBrowserDialog.SelectedPath;
            Program.Settings.DirectoryList.AddDirectory(pathToAdd);
            this.foldersListBox_DataChanged();
            Program.Settings.SaveSettings();
        }

        private void removeDirectoryButton_Click(object sender, EventArgs e)
        {
            this.RemoveDirectory();
        }

        private void refreshDirectoryButton_Click(object sender, EventArgs e)
        {
            if (this.foldersListBox.SelectedItem != null)
            {
                var directory = (Directory)this.foldersListBox.SelectedItem;
                directory.Refresh();
                this.SelectDirectory();
                Program.Settings.SaveSettings();
            }
        }

        private void compileSelectedButton_Click(object sender, EventArgs e)
        {
            this.CompileSelectedFiles();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CompileSelectedFiles();
        }

        private void compileResultsDataGridView_DataChanged()
        {
            ((CurrencyManager)this.compileResultsDataGridView.BindingContext[this.compileResultsDataGridView.DataSource]).Refresh();
        }

        private void clearCompileResultsButton_Click(object sender, EventArgs e)
        {
            this.compileResultsDataGridView.DataSource = new List<CompileCommandResult>();
            this.compileResultsDataGridView_DataChanged();
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.RestoreFromTray();
            }
            else
            {
                this.MinimizeToTray();
            }
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.RestoreFromTray();
            this.tabControl.SelectTab(this.compilerTabPage);
        }

        private void notifyIconMenuOpen_Click(object sender, EventArgs e)
        {
            this.RestoreFromTray();
        }

        private void notifyIconMenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            form.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AboutForm();
            form.ShowDialog(this);
        }

        #endregion Event Handlers

        #region Public Methods

        public void AddCompileResult(CompileCommandResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddCompileResultDelegate(this.AddCompileResult), result);
                return;
            }

            if (result.IsSuccess)
            {
                result.ResultText = "success";
            }

            var compileResults = (List<CompileCommandResult>)this.compileResultsDataGridView.DataSource;
            compileResults.Insert(0, result);
            this.compileResultsDataGridView_DataChanged();

            if (result.IsSuccess && Program.Settings.ShowSuccessMessages)
            {
                this.ShowSuccessNotification("Successful compile", result.ResultText);
            }
            else if (!result.IsSuccess)
            {
                this.ShowErrorNotification("Compile error", result.ResultText);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void LoadDirectories(CommandArguments args)
        {
            if (args.ClearDirectories)
            {
                Program.Settings.DirectoryList.ClearDirectories();
            }

            //load directories specified in arguments
            foreach (string directoryPath in args.DirectoryPaths)
            {
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    continue;
                }

                Directory directory = Program.Settings.DirectoryList.AddDirectory(directoryPath);

                foreach (File file in directory.Files)
                {
                    file.Minify = args.Minify;
                    if (args.InitialCompile)
                    {
                        file.Compile();
                    }
                }
            }

            this.foldersListBox_DataChanged();
            this.SelectDirectory();
            Program.Settings.SaveSettings();
        }

        private void RemoveDirectory()
        {
            if (this.foldersListBox.SelectedItem != null)
            {
                Program.Settings.DirectoryList.RemoveDirectory((Directory)this.foldersListBox.SelectedItem);
                this.foldersListBox_DataChanged();
                this.filesDataGridView.DataSource = new List<File>();
                this.filesDataGridView_DataChanged();
                Program.Settings.SaveSettings();
            }
        }

        private void SelectDirectory()
        {
            var directory = (Directory)this.foldersListBox.SelectedItem;
            if (directory != null)
            {
                this.filesDataGridView.DataSource = directory.Files;
            }
            else
            {
                this.filesDataGridView.DataSource = new List<File>();
            }

            this.filesDataGridView_DataChanged();
        }

        private void MinimizeToTray()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ShowSuccessNotification(string title, string message)
        {
            this.notifyIcon.ShowBalloonTip(500, title, message, ToolTipIcon.Info);
        }

        private void ShowErrorNotification(string title, string message)
        {
            this.notifyIcon.ShowBalloonTip(500, title, message, ToolTipIcon.Error);
        }

        private void CompileSelectedFiles()
        {
            // Retrieve list of files from data grid
            var files = (List<File>)this.filesDataGridView.DataSource;

            // Compile files one by one
            foreach (File file in files)
            {
                if (file.Enabled)
                {
                    LessCompiler.Compile(file.FullPath, file.OutputPath, file.Minify);
                }
            }
        }

        #endregion Private Methods
    }
}
