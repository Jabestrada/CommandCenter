using System;
using System.Diagnostics;
using System.IO;

namespace CommandCenter.Commands.CmdLine {
    public sealed class CommandLineProcessRunner : IDisposable {
        public string Path { get; }
        public string Arguments { get; }
        public bool IsRunning { get; private set; }
        public int? ExitCode { get; private set; }

        private Process _process;
        private readonly object Locker = new object();

        public CommandLineProcessRunner(string path, string arguments) {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException($"Executable not found: {path}");
            Arguments = arguments;
        }

        public int Run(Action<string> outputStreamCallback, Action<string> errorStreamCallback) {
            lock (Locker) {
                if (IsRunning) throw new Exception("The process is already running");

                _process = new Process() {
                    EnableRaisingEvents = true,
                    StartInfo = new ProcessStartInfo() {
                        FileName = Path,
                        Arguments = Arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    },
                };

                if (outputStreamCallback != null) {
                    _process.OutputDataReceived += (sender, args) => outputStreamCallback(args.Data);
                }
                if (errorStreamCallback != null) {
                    _process.ErrorDataReceived += (sender, args) => errorStreamCallback(args.Data);
                }

                if (!_process.Start()) throw new Exception("Process could not be started");

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
                
                _process.WaitForExit();
                try { _process.Refresh(); } catch { }
                return (ExitCode = _process.ExitCode).Value;
            }
        }

        public void Kill() {
            lock (Locker) {
                try { _process?.Kill(); }
                catch { }
                IsRunning = false;
                _process = null;
            }
        }

        public void Dispose() {
            try { _process?.Dispose(); }
            catch { }
        }
    }
}
