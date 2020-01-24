namespace WinLessCore.Models
{
    public class CompileCommandResult : CommandResult
    {
        public CompileCommandResult(CommandResult result)
        {
            this.Time = result.Time;
            this.IsSuccess = result.IsSuccess;
            this.ResultText = result.ResultText;
        }

        public string FullPath { get; set; }
    }
}
