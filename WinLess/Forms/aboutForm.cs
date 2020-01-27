﻿using System.Windows.Forms;

namespace WinLessCore.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.InitializeComponent();

            this.winlessVersionLabel.Text = this.GetApplicationVersion();
            this.lessjsVersionLabel.Text = LessCompiler.GetCurrentCompilerVersion().ToString();
        }

        private string GetApplicationVersion()
        {
            string version = Application.ProductVersion;

            // return the ProductVersion without the last '.0'
            return version[..^2];
        }
    }
}
