using CommandCenter.Infrastructure;
using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Windows.Forms;

namespace CommandCenter.Commands.WinForms {
    public class ConditionalPauseCommand : BaseCommand {
        public string[] PromptTexts { get; protected set; }
        public override bool IsUndoable => false;
        public ConditionalPauseCommand(params string[] linesOfText) {
            PromptTexts = linesOfText;
        }
        public override void Do() {
            SendReport(this, $"ConditionalPause => User dialog displayed. Awaiting user input...", ReportType.Progress);
            var userReply = MessageBox.Show(string.Join(Environment.NewLine, PromptTexts), "User Input Needed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            DidCommandSucceed = userReply == DialogResult.Yes;
            var cmdResult = DidCommandSucceed ? "succeeded" : "failed";
            SendReport(this, $"ConditionalPause => Command {cmdResult} because user responded with {userReply}",
                       DidCommandSucceed ? ReportType.DoneTaskWithSuccess : ReportType.DoneTaskWithFailure);
        }
    }
}
