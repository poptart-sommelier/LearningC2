using System;
using System.Runtime.InteropServices;

namespace Agent.Native
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(
           string lpApplicationName,
           string lpCommandLine,
           ref SECURITY_ATTRIBUTES lpProcessAttributes,
           ref SECURITY_ATTRIBUTES lpThreadAttributes,
           bool bInheritHandles,
           CreationFlags dwCreationFlags,
           IntPtr lpEnvironment,
           string lpCurrentDirectory,
           ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32")]
        public static extern IntPtr CreateThread(
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            out IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            out IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            int dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            int dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            int dwSize,
            MemoryProtection flNewProtect,
            out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(
            IntPtr lpAddress,
            int dwSize,
            MemoryProtection flNewProtect,
            out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern uint QueueUserAPC(
            IntPtr pfnAPC,
            IntPtr hThread,
            uint dwData);

        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process(
            IntPtr processHandle,
            out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
        ProcessAccess processAccess,
            bool bInheritHandle,
            int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess,
            bool bInheritHandle,
            uint dwThreadId);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
        }

        [Flags]
        public enum CreationFlags : uint
        {
            CreateSuspended = 0x00000004,
            DetachedProcesds = 0x00000008,
            CreateNoWindow = 0x08000000,
            ExtendedStartupInfoPresent = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public uint cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttributes;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdErr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [Flags]
        public enum ProcessAccess
        {
            All = 0x001FFFFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum ThreadAccess
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            QUERY_LIMITED = (0x00000800),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }        
    }
}
