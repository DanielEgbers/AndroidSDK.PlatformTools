using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools
{
    public class WrappedProcess : Process
    {
        public WrappedProcess(string path, params string[] arguments) : 
            this(path, string.Join(" ", arguments.Select(arg => arg.Contains(" ") ? $"\"{arg}\"" : arg)))
        {

        }

        public WrappedProcess(string path, string arguments) : 
            base()
        {
            StartInfo = new ProcessStartInfo(path, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
        }

        public new void Start()
        {
            throw new NotSupportedException($"use '{nameof(StartAsync)}'");
        }
        
        public async Task StartAsync(IProgress<string> progress = null, CancellationToken? cancellationToken = null)
        {
            //await StartLoopBasedAsync(progress, cancellationToken, breakawayChildProcess: false);
            await StartEventBasedAsync(progress, cancellationToken, breakawayChildProcess: false);
        }

        private async Task StartLoopBasedAsync(IProgress<string> progress = null, CancellationToken? cancellationToken = null, bool breakawayChildProcess = false)
        {
            await Task.Yield();

            void HandleDataReceived(string data)
            {
                progress?.Report(data);
            }

            var jobHandle = this.CreateKillOnCurrentProcessExitJob(breakawayChildProcess: breakawayChildProcess);

            base.Start();

            this.KillOnCurrentProcessExit(jobHandle);

            while (NextOutputLine(out var outputLine) && cancellationToken?.IsCancellationRequested != true)
            {
                HandleDataReceived(outputLine);
            }

            bool NextOutputLine(out string outputLine)
            {
                if (StandardError?.EndOfStream == false)
                {
                    outputLine = StandardError.ReadLine();
                    return true;
                }
                else if (StandardOutput?.EndOfStream == false)
                {
                    outputLine = StandardOutput.ReadLine();
                    return true;
                }
                else
                {
                    outputLine = null;
                    return false;
                }
            }
        }

        private async Task StartEventBasedAsync(IProgress<string> progress = null, CancellationToken? cancellationToken = null, bool breakawayChildProcess = false)
        {
            var completionErrorData = new TaskCompletionSource<bool>();
            var completionOutputData = new TaskCompletionSource<bool>();

            void HandleDataReceived(string data, TaskCompletionSource<bool> completion)
            {
                if (data == null)
                    completion.SetResult(true);
                else
                    progress?.Report(data);
            }

            void HandleErrorDataReceived(object s, DataReceivedEventArgs a)
            {
                HandleDataReceived(a.Data, completionErrorData);
            }

            void HandleOutputDataReceived(object s, DataReceivedEventArgs a)
            {
                HandleDataReceived(a.Data, completionOutputData);
            }

            ErrorDataReceived += HandleErrorDataReceived;
            OutputDataReceived += HandleOutputDataReceived;
            try
            {
                var jobHandle = this.CreateKillOnCurrentProcessExitJob(breakawayChildProcess: breakawayChildProcess);

                base.Start();

                this.KillOnCurrentProcessExit(jobHandle);

                BeginErrorReadLine();
                BeginOutputReadLine();

                if (cancellationToken != null)
                    await Task.WhenAny(Task.WhenAll(completionErrorData.Task, completionOutputData.Task), cancellationToken.AsTask());
                else
                    await Task.WhenAll(completionErrorData.Task, completionOutputData.Task);
            }
            finally
            {
                OutputDataReceived -= HandleOutputDataReceived;
                ErrorDataReceived -= HandleErrorDataReceived;
            }
        }
    }
}
