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
using System.Runtime.InteropServices;
using System.Security.Principal;
using static Agent.Native.Advapi;
using static Agent.Native.Winnt;
using static Agent.Native.Kernel32;

namespace Agent.Internal
{
    public class TokenInfo
    {
        public static string GetProcessIntegrityLevel(Process process)
        {
            IntPtr processHandle = process.Handle;
            IntPtr tokenHandle = IntPtr.Zero;
            bool result;

            if (!OpenProcessToken(processHandle, (uint)TokenAccess.TOKEN_QUERY_ALL, out tokenHandle)) {
                throw new ArgumentException("Could not open process token");
            }

            string integrityLevel = GetTokenInfo(tokenHandle);

            result = CloseHandle(tokenHandle);
            result = CloseHandle(processHandle);

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

            GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, tokenLength, ref tokenLength);

            tokenPtr = Marshal.AllocHGlobal((IntPtr)tokenLength);

            if (!GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, tokenPtr, tokenLength, ref tokenLength))
            {
                throw new ArgumentException("Could not get token information");
            }
            else
            {
                var tokenIntegrityLevel = (TOKEN_MANDATORY_LABEL)Marshal.PtrToStructure(tokenPtr, typeof(TOKEN_MANDATORY_LABEL));

                if (!ConvertSidToStringSid(tokenIntegrityLevel.Label.Sid, ref stringPtr))
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