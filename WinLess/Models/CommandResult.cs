using System;

namespace WinLessCore.Models
{
    public class CommandResult
    {
        public CommandResult()
        {
            this.IsSuccess = false;
            this.ResultText = string.Empty;
        }

        public DateTime Time { get; set; }

        public bool IsSuccess { get; set; }

        public string ResultText { get; set; }
    }
}
