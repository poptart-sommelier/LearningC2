using Agent.Models;
using SharpSploit.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;

namespace Agent.Commands
{
    public class ListProcesses : AgentCommand
    {
        public override string Name => "ps";

        public override string Execute(AgentTask task)
        {
            var results = new SSResultList<ListProcessesResult>();
            var processes = Process.GetProcesses();
     
            foreach (var process in processes)
            {
                var result = new ListProcessesResult
                {
                    ProcessName = process.ProcessName,
                    ProcessId = process.Id,
                    SessionId = process.SessionId
                };

                result.ProcessPath = GetProcessPath(process);
                result.Owner = GetProcessOwner(process);
                result.Arch = GetProcessArch(process);
                results.Add(result);
            }

            return results.ToString();
        }

        private string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                return "-";
            }
        }

        private string GetProcessOwner(Process process)
        {
            var hToken = IntPtr.Zero;

            try
            {
                if (!Native.Advapi.OpenProcessToken(process.Handle, (uint)Native.Advapi.DesiredAccess.TOKEN_ALL_ACCESS, out hToken))
                    return "-";

                var identity = new WindowsIdentity(hToken);
                return identity.Name;
            }
            catch
            {
                return "-";
            }
            finally
            {
                Native.Kernel32.CloseHandle(hToken);
            }
        }

        private string GetProcessArch(Process process)
        {
            try
            {
                var is64BitOS = Environment.Is64BitOperatingSystem;

                if (!is64BitOS)
                    return "x86";

                if (!Native.Kernel32.IsWow64Process(process.Handle, out var isWow64))
                    return "-";

                if (is64BitOS && isWow64)
                    return "x86";

                return "x64";
            }
            catch
            {
                return "-";
            }
        }
    }
    public sealed class ListProcessesResult : SSResult
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public string Owner { get; set; }
        public int ProcessId { get; set; }
        public int SessionId { get; set; }
        public string Arch { get; set; }

        protected internal override IList<SSResultProperty> ResultProperties => new List<SSResultProperty>
        {
            new SSResultProperty{Name = nameof(ProcessName), Value = ProcessName },
            new SSResultProperty{Name = nameof(ProcessPath), Value = ProcessPath },
            new SSResultProperty{Name = nameof(Owner), Value = Owner},
            new SSResultProperty{Name = "PID", Value = ProcessId },
            new SSResultProperty{Name = nameof(SessionId), Value = SessionId },
            new SSResultProperty{Name = nameof(Arch), Value = Arch }
        };
    }
}

