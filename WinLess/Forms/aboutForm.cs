﻿using System.Windows.Forms;

namespace WinLess
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            winlessVersionLabel.Text = GetApplicationVersion();
            lessjsVersionLabel.Text = LessCompiler.GetCurrentCompilerVersion().ToString();
        }

        private string GetApplicationVersion()
        {
            string version = Application.ProductVersion;

            // return the ProductVersion without the last '.0'
            return version.Substring(0, version.Length - 2);
        }
    }
}