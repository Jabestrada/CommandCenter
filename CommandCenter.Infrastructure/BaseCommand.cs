using CommandCenter.Infrastructure.Orchestration;
using System;
using System.Security.Principal;

namespace CommandCenter.Infrastructure {
    public interface ICommand {
        void Do();
        void Undo();

        bool IsUndoable { get; }

        void Cleanup();

        bool HasPreFlightCheck { get; }

        bool PreflightCheck();


        // Feature candidate
        //bool IsCancellable { get; }

        // Potentially useful properties when support for command-nesting comes up.
        //bool IsFullyUndoable { get; }
        //bool IsFullyCancellable { get; }
    }

    public abstract class BaseCommand : ICommand {
        #region ICommand
        public virtual bool HasPreFlightCheck => false;
        public abstract bool IsUndoable { get; }

        public abstract void Do();
        public virtual void Undo() {
            throw new NotImplementedException();
        }
        public virtual void Cleanup() { 
            // Empty default implementation
        }
        public virtual bool PreflightCheck() { 
            SendReport(this, $"{ShortName} pre-flight checks done and it is likely to succeed", ReportType.DonePreflightWithSuccess);
            return true;
        }
        #endregion

        public readonly string Id;
        public bool DidCommandSucceed { get; protected set; }
        public bool WasCommandStarted { get; internal set; }
        public bool Enabled { get; set; }
        public string ShortDescription { get; set; }

        private string _shortName;
        public virtual string ShortName { 
            get {
                if (_shortName == null) {
                    var typeName = GetType().ToString();
                    _shortName = typeName.Substring(typeName.LastIndexOf('.') + 1);
                }
                return _shortName;
            } 
        }
        public BaseCommand() {
            Id = Guid.NewGuid().ToString();
            Enabled = true;
            ShortDescription = string.Empty;
        }

        public delegate void ReportSentEventHandler(BaseCommand command, CommandReportArgs args);
        public event ReportSentEventHandler OnReportSent;

        protected void SendReport(BaseCommand command, CommandReportArgs args) {
            OnReportSent?.Invoke(command, args);
        }
        protected void SendReport(BaseCommand command, string message, ReportType reportType) {
            SendReport(command, new CommandReportArgs(message, reportType));
        }
        protected void SendReport(string message, ReportType reportType) {
            SendReport(this, new CommandReportArgs(message, reportType));
        }

       protected bool IsCurrentUserAdmin {
            get {
                try {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                catch {
                    return false;
                }

            }
        }
    }

    public class CommandReportArgs : EventArgs {
        public readonly ReportType ReportType;
        public readonly string Message;
        public double ProgressPercentage;
        public CommandReportArgs(string message, ReportType type, double progressPercentage) {
            Message = message;
            ReportType = type;
            ProgressPercentage = progressPercentage;
        }

        public CommandReportArgs(string message, ReportType type) : this(message, type, 0) {

        }

    }


}
