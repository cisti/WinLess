using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WinLess;
using WinLessCore.Helpers;
using WinLessCore.Models;

namespace WinLessCore
{
    public static class LessCompiler
    {
        public static void Compile(string lessFile, string cssFile, bool minify)
        {
            try
            {
                CompileCommandResult compileResult = ExecuteCompileCommand(lessFile, cssFile, minify);
                MainForm.ActiveOrInActiveMainForm.AddCompileResult(compileResult);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
        }

        public static Version GetCurrentCompilerVersion()
        {
            CommandResult result = ExecuteLessCommand("-v");

            return GetVersionFromCommandResult(result);
        }

        private static Version GetVersionFromCommandResult(CommandResult result)
        {
            if (!result.IsSuccess)
            {
                return null;
            }

            Match versionMatch = Regex.Match(result.ResultText, "\\d+(?:\\.\\d+)+");

            if (versionMatch.Groups.Count > 0)
            {
                return new Version(versionMatch.Groups[0].Value);
            }

            return null;
        }

        private static CommandResult ExecuteLessCommand(string arguments)
        {
            string fileName = Program.Settings.UseGloballyInstalledLess ? $@"{Application.StartupPath}\lessc.cmd" : $@"{Application.StartupPath}\node_modules\.bin\lessc.cmd";
            return ExecuteCommand(fileName, arguments);
        }

        private static CompileCommandResult ExecuteCompileCommand(string lessFile, string cssFile, bool minify)
        {
            string arguments = CreateCompileArguments(lessFile, cssFile, minify);

            var result = new CompileCommandResult(ExecuteLessCommand(arguments))
            {
                FullPath = lessFile
            };

            return result;
        }

        private static string CreateCompileArguments(string lessFile, string cssFile, bool minify)
        {
            string arguments = $@"""{lessFile}"" ""{cssFile}"" --no-color";

            if (minify)
            {
                arguments = $@"{arguments} --clean-css=""--compatibility=ie8 --advanced""";
            }

            return arguments;
        }

        private static CommandResult ExecuteCommand(string fileName, string arguments)
        {
            var result = new CommandResult();

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Application.StartupPath,
                    FileName = fileName,
                    Arguments = arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                var process = new Process { StartInfo = startInfo };
                process.Start();

                string error = process.StandardError.ReadToEnd();

                if (error.Length > 0)
                {
                    result.ResultText = error;
                }
                else
                {
                    result.ResultText = process.StandardOutput.ReadToEnd();
                    result.IsSuccess = true;
                }

                process.WaitForExit();
                result.Time = DateTime.Now;

                return result;
            }
            catch (Exception e)
            {
                result.Time = DateTime.Now;
                result.IsSuccess = false;
                result.ResultText = e.Message;
            }

            return result;
        }
    }
}
