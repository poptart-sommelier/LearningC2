/* Gets Process Integrity Level

references: 
https://redcanary.com/blog/process-integrity-levels/
https://gist.github.com/jsecurity101/5ef14a0b537af36ce448b28c707c6976
https://www.pinvoke.net/default.aspx/advapi32.openprocesstoken
https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process?view=net-7.0
https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.getcurrentprocess?view=net-7.0#system-diagnostics-process-getcurrentprocess
https://devblogs.microsoft.com/oldnewthing/20221017-00/?p=107291#:~:text=You%20can%20inspect%20a%20process%E2%80%99s%20integrity%20level%20by,process%20token%20%28and%20then%20closing%20it%20when%20done%29.
https://devblogs.microsoft.com/oldnewthing/20210105-00/?p=104667

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TokenInformation
{
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

    [Flags]
    public enum TokenAccess
    {
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        STANDARD_RIGHTS_READ = 0x00020000,
        TOKEN_ASSIGN_PRIMARY = 0x0001,
        TOKEN_DUPLICATE = 0x0002,
        TOKEN_IMPERSONATE = 0x0004,
        TOKEN_QUERY = 0x0008,
        TOKEN_QUERY_SOURCE = 0x0010,
        TOKEN_ADJUST_PRIVILEGES = 0x0020,
        TOKEN_ADJUST_GROUPS = 0x0040,
        TOKEN_ADJUST_DEFAULT = 0x0080,
        TOKEN_ADJUST_SESSIONID = 0x0100,
        TOKEN_IMPERSONATEUSER = (TOKEN_DUPLICATE | TOKEN_QUERY),
        TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
        TOKEN_QUERY_ALL = (TOKEN_QUERY | TOKEN_QUERY_SOURCE),
        TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID)
    }
    [Flags]
    public enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel
    }

    public enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    }

    [Flags]
    public enum TOKEN_TYPE
    {
        TokenPrimary = 1,
        TokenImpersonation = 2
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TOKEN_STATISTICS
    {
        public LUID TokenId;
        public LUID AuthenticationId;
        public long ExpirationTime;
        public uint TokenType;
        public uint ImpersonationLevel;
        public uint DynamicCharged;
        public uint DynamicAvailable;
        public uint GroupCount;
        public uint PrivilegeCount;
        public LUID ModifiedId;
    }

    public struct TOKEN_USER
    {
        public SID_AND_ATTRIBUTES User;
    }

    public struct TOKEN_OWNER
    {
        public IntPtr Sid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SID_AND_ATTRIBUTES
    {

        public IntPtr Sid;
        public int Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        public UInt32 LowPart;
        public Int32 HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_ORIGIN
    {
        public LUID OriginatingLogonSession;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_MANDATORY_LABEL
    {
        public SID_AND_ATTRIBUTES Label;
    }

    public class ProcessNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccess processAccess,
            bool bInheritHandle,
            int processId);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            uint DesiredAccess,
            ref IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            ref uint ReturnLength);

        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ConvertSidToStringSid(
            IntPtr pSid,
            ref string strSid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ImpersonateLoggedOnUser(
            IntPtr hToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(
            IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(
            ThreadAccess dwDesiredAccess,
            bool bInheritHandle,
            uint dwThreadId);


        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenThreadToken(
            IntPtr ThreadHandle,
            uint DesiredAccess,
            bool OpenAsSelf,
            ref IntPtr TokenHandle);

        [DllImport("advapi32.dll")]
        public extern static bool DuplicateToken(
            IntPtr ExistingTokenHandle,
            int SECURITY_IMPERSONATION_LEVEL,
            ref IntPtr DuplicateTokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();
    }

    public class TokenInfo
    {
        public static string GetProcessIntegrityLevel(Process process)
        {
            IntPtr processHandle = process.Handle;
            IntPtr tokenHandle = IntPtr.Zero;
            bool result;

            if (!ProcessNativeMethods.OpenProcessToken(processHandle, (uint)TokenAccess.TOKEN_QUERY_ALL, ref tokenHandle)) {
                throw new ArgumentException("Could not open process token");
            }

            string integrityLevel = GetTokenInfo(tokenHandle);

            result = ProcessNativeMethods.CloseHandle(tokenHandle);
            result = ProcessNativeMethods.CloseHandle(processHandle);

            return integrityLevel;
        }

        public static string GetTokenInfo(IntPtr tokenHandle)
        {
            uint tokenLength = 0;
            IntPtr tokenPtr = IntPtr.Zero;
            string stringPtr = "";

            // Reference: https://docs.microsoft.com/en-US/windows/security/identity-protection/access-control/security-identifiers
            var sidToIntegrity = new Dictionary<string, string>()
            {
                {"S-1-16-0" , "UNTRUSTED_MANDATORY_LEVEL"},
                {"S-1-16-4096", "LOW_MANDATORY_LEVEL"},
                {"S-1-16-8192", "MEDIUM_MANDATORY_LEVEL"},
                {"S-1-16-8448", "MEDIUM_PLUS_MANDATORY_LEVEL"},
                {"S-1-16-12288", "HIGH_MANDATORY_LEVEL"},
                {"S-1-16-16384", "SYSTEM_MANDATORY_LEVEL"},
                {"S-1-16-20480", "PROTECTED_PROCESS_MANDATORY_LEVEL"},
                {"S-1-16-28672", "SECURE_PROCESS_MANDATORY_LEVEL"}
            };

            // result = ProcessNativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, tokenLength, ref tokenLength);
            ProcessNativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, tokenLength, ref tokenLength);

            tokenPtr = Marshal.AllocHGlobal((IntPtr)tokenLength);

            if (!ProcessNativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, tokenPtr, tokenLength, ref tokenLength))
            {
                throw new ArgumentException("Could not get token information");
            }
            else
            {
                var tokenIntegrityLevel = (TOKEN_MANDATORY_LABEL)Marshal.PtrToStructure(tokenPtr, typeof(TOKEN_MANDATORY_LABEL));

                if (!ProcessNativeMethods.ConvertSidToStringSid(tokenIntegrityLevel.Label.Sid, ref stringPtr))
                {
                    throw new ArgumentException("Could not convert SID to string");
                }
                else
                {
                    var tokenIntegritySID = new SecurityIdentifier(stringPtr);
                    return sidToIntegrity[tokenIntegritySID.Value];
                }
            }
        }   
    }
}