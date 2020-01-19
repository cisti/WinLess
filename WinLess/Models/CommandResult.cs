using System;

namespace WinLess.Models
{
    public class CommandResult
    {
        public CommandResult()
        {
            IsSuccess = false;
            ResultText = "";
        }

        public string TimeString => Time.ToLongTimeString();

        public DateTime Time
        {
            get;
            set;
        }

        public bool IsSuccess
        {
            get;
            set;
        }

        public string ResultText
        {
            get;
            set;
        }
    }
}