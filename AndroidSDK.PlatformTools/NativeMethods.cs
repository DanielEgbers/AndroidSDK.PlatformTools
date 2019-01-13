using System;
using System.Runtime.InteropServices;

namespace AndroidSDK.PlatformTools
{

#pragma warning disable IDE1006 // Benennungsstile
#pragma warning disable PC003 // Native API not available in UWP

    internal static class NativeMethods
    {
        public enum JobObjectInfoType
        {
            AssociateCompletionPortInformation = 7,
            BasicLimitInformation = 2,
            BasicUIRestrictions = 4,
            EndOfJobTimeInformation = 6,
            ExtendedLimitInformation = 9,
            SecurityLimitInformation = 5,
            GroupInformation = 11
        }

        [Flags]
        public enum JOBOBJECTLIMIT : uint
        {
            JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x00000008,
            JOB_OBJECT_LIMIT_AFFINITY = 0x00000010,
            JOB_OBJECT_LIMIT_BREAKAWAY_OK = 0x00000800,
            JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION = 0x00000400,
            JOB_OBJECT_LIMIT_JOB_MEMORY = 0x00000200,
            JOB_OBJECT_LIMIT_JOB_TIME = 0x00000004,
            JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000,
            JOB_OBJECT_LIMIT_PRESERVE_JOB_TIME = 0x00000040,
            JOB_OBJECT_LIMIT_PRIORITY_CLASS = 0x00000020,
            JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x00000100,
            JOB_OBJECT_LIMIT_PROCESS_TIME = 0x00000002,
            JOB_OBJECT_LIMIT_SCHEDULING_CLASS = 0x00000080,
            JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK = 0x00001000,
            JOB_OBJECT_LIMIT_SUBSET_AFFINITY = 0x00004000,
            JOB_OBJECT_LIMIT_WORKINGSET = 0x00000001
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public JOBOBJECTLIMIT LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public long Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string name);

        [DllImport("kernel32.dll")]
        public static extern bool SetInformationJobObject(IntPtr job, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
    }

#pragma warning restore PC003 // Native API not available in UWP
#pragma warning restore IDE1006 // Benennungsstile

}
