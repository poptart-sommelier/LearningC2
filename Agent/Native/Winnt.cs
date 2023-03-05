using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Native
{
    public static class Winnt
    {
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
    }
}
