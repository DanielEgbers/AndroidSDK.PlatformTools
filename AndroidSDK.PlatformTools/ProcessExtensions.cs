using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools
{
    public static class ProcessExtensions
    {
        public static IntPtr KillOnCurrentProcessExit(this Process process, bool allowChildProcessBreakaway = false, bool breakawayChildProcess = false)
        {
            var jobHandle = CreateKillOnCurrentProcessExitJob(process, allowChildProcessBreakaway, breakawayChildProcess);
            KillOnCurrentProcessExit(process, jobHandle);
            return jobHandle;
        }

        public static void KillOnCurrentProcessExit(this Process process, IntPtr jobHandle)
        {
            if (process.Handle == IntPtr.Zero)
                throw new InvalidOperationException();

            if (!NativeMethods.AssignProcessToJobObject(jobHandle, process.Handle) && !process.HasExited)
                throw new Win32Exception();
        }

        // https://stackoverflow.com/a/37034966
        public static IntPtr CreateKillOnCurrentProcessExitJob(this Process process, bool allowChildProcessBreakaway = false, bool breakawayChildProcess = false)
        {
            var currentProcess = Process.GetCurrentProcess();
            var currentProcessInfo = $"{currentProcess.ProcessName}_{currentProcess.Id}_{currentProcess.Handle}";
            var processInfo = process.StartInfo?.FileName != null ? $"{Path.GetFileNameWithoutExtension(process.StartInfo?.FileName)}" : null;
            var jobName = $"{nameof(KillOnCurrentProcessExit)}___{currentProcessInfo}{(processInfo != null ? $"__{processInfo}" : string.Empty)}___{Guid.NewGuid().ToString("N")}";

            var limit = NativeMethods.JOBOBJECTLIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
            if (allowChildProcessBreakaway)
            {
                limit |= NativeMethods.JOBOBJECTLIMIT.JOB_OBJECT_LIMIT_BREAKAWAY_OK;
            }
            if (breakawayChildProcess)
            {
                limit |= NativeMethods.JOBOBJECTLIMIT.JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK;
            }

            var extendedInfo = new NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = new NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION
                {
                    LimitFlags = limit
                }
            };

            var extendedInfoSize = Marshal.SizeOf(typeof(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION));

            var extendedInfoPtr = Marshal.AllocHGlobal(extendedInfoSize);
            try
            {
                Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

                var jobHandle = NativeMethods.CreateJobObject(IntPtr.Zero, jobName);

                if (!NativeMethods.SetInformationJobObject(jobHandle, NativeMethods.JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)extendedInfoSize))
                    throw new Win32Exception();

                return jobHandle;
            }
            finally
            {
                Marshal.FreeHGlobal(extendedInfoPtr);
            }
        }

        public static Process SetStartInfo(this Process process, ProcessStartInfo startInfo)
        {
            process.StartInfo = startInfo;
            return process;
        }

        public static Process SetStartInfo(this Process process, string path, string[] arguments = null)
        {
            if (process.StartInfo == null)
            {
                process.StartInfo = new ProcessStartInfo();
            }

            if (path != null)
            {
                process.StartInfo.FileName = path;
            }
            if (arguments != null)
            {
                process.StartInfo.Arguments = string.Join(" ", arguments.Select(arg => arg.Contains(" ") ? $"\"{arg}\"" : arg));
            }

            return process;
        }

        public static async Task StartAsync(this Process process, bool allowChildProcessBreakaway = false, bool breakawayChildProcess = false, CancellationToken? cancellationToken = null)
        {
#if DEBUG
            Console.WriteLine($"> {Path.GetFileNameWithoutExtension(process.StartInfo.FileName)} {process.StartInfo.Arguments}");
#endif
            await Task.Yield();

            var completion = new TaskCompletionSource<bool>();

            void HandleExited(object s, EventArgs a)
            {
                completion.SetResult(true);
            }

            process.EnableRaisingEvents = true;

            process.Exited += HandleExited;
            try
            {
                var jobHandle = process.CreateKillOnCurrentProcessExitJob
                (
                    allowChildProcessBreakaway: allowChildProcessBreakaway,
                    breakawayChildProcess: breakawayChildProcess
                );

                process.Start();

                process.KillOnCurrentProcessExit(jobHandle);

                if (cancellationToken != null)
                    await Task.WhenAny(completion.Task, cancellationToken.AsTask());
                else
                    await completion.Task;
            }
            finally
            {
                process.Exited -= HandleExited;
            }
        }
    }
}
