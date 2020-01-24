using System;
using System.IO;
using System.Windows.Forms;

namespace WinLessCore.Helpers
{
    internal static class ExceptionHandler
    {
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }

        public static void ShowException(string message, Exception exception)
        {
            ShowErrorMessage($@"{message}Exception message:{exception.Message}");
        }

        public static void LogErrorMessage(string message)
        {
            try
            {
                string errorFile = $@"{Application.CommonAppDataPath}\data\errors.txt";
                TextWriter writer = File.AppendText(errorFile);
                writer.WriteLine(message);
                writer.Flush();
                writer.Close();
            }
            catch
            {
                // do nothing
            }
        }

        public static void LogException(Exception exception)
        {
            string errorMessage = $@"{DateTime.Now} Message: {exception.Message} Source: {exception.Source} StackTrace: {exception.StackTrace}";
            LogErrorMessage(errorMessage);
        }
    }
}
