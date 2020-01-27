using System;
using System.IO;
using System.Windows.Forms;
using WinLessCore.Helpers;
using WinLessCore.Models;

namespace WinLessCore
{
    public class Settings
    {
        public bool CompileOnSave { get; set; }

        public bool DefaultMinify { get; set; }

        public bool ShowSuccessMessages { get; set; }

        public bool StartMinified { get; set; }

        public bool UseGloballyInstalledLess { get; set; }

        public DirectoryList DirectoryList { get; set; }

        private bool startWithWindows;

        public bool StartWithWindows
        {
            get => this.startWithWindows;
            set
            {
                this.startWithWindows = value;
                this.ApplyStartWithWindows();
            }
        }

        public Settings()
        {
            this.CompileOnSave = true;
            this.DefaultMinify = true;
            this.ShowSuccessMessages = false;
            this.StartMinified = false;
            this.StartWithWindows = true;
            this.UseGloballyInstalledLess = false;

            this.DirectoryList = new DirectoryList();
        }

        public void SaveSettings()
        {
            string dataDir = $@"{Application.UserAppDataPath}\data";
            string settingsFilePath = $@"{dataDir}\settings.xml";

            try
            {
                if (!System.IO.Directory.Exists(dataDir))
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.ShowErrorMessage($"Error while trying to create data directory: {dataDir}\n\nException message:\n{e.Message}");
            }

            try
            {
                TextWriter writer = new StreamWriter(settingsFilePath);
                var serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch (Exception e)
            {
                ExceptionHandler.ShowErrorMessage($"Error while trying to save settings: {settingsFilePath}\n\nException message:\n{e.Message}");
            }
        }

        public static Settings LoadSettings()
        {
            string path = $@"{Application.UserAppDataPath}\data\settings.xml";

            if (!System.IO.File.Exists(path))
            {
                return new Settings();
            }

            try
            {
                TextReader reader = new StreamReader(path);
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                var settings = (Settings)serializer.Deserialize(reader);
                reader.Close();
                return settings;
            }
            catch
            {
                return new Settings();
            }
        }

        private void ApplyStartWithWindows()
        {
            const string keyName = "WinLess";
            string assemblyLocation = Application.ExecutablePath;

            if (this.StartWithWindows && !AutoStartUtil.IsAutoStartEnabled(keyName, assemblyLocation))
            {
                AutoStartUtil.SetAutoStart(keyName, assemblyLocation);
            }

            if (!this.StartWithWindows && AutoStartUtil.IsAutoStartEnabled(keyName, assemblyLocation))
            {
                AutoStartUtil.UnSetAutoStart(keyName);
            }
        }
    }
}
