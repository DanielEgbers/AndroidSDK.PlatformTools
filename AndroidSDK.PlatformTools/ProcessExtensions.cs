using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

// https://stackoverflow.com/a/37034966

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

            if (!NativeMethods.AssignProcessToJobObject(jobHandle, process.Handle))
                throw new Win32Exception();
        }

        public static IntPtr CreateKillOnCurrentProcessExitJob(this Process process, bool allowChildProcessBreakaway = false, bool breakawayChildProcess = false)
        {
            var currentProcess = Process.GetCurrentProcess();

            var jobName = $"{nameof(KillOnCurrentProcessExit)}___{currentProcess.ProcessName}_{currentProcess.Id}_{currentProcess.Handle}___{Guid.NewGuid().ToString("N")}";

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
    }
}
