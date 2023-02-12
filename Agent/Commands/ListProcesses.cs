using Agent.Models;
using SharpSploit.Generic;
using System.Collections.Generic;
using System.Diagnostics;

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
    }
    public sealed class ListProcessesResult : SSResult
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public int ProcessId { get; set; }
        public int SessionId { get; set; }

        protected internal override IList<SSResultProperty> ResultProperties => new List<SSResultProperty>
        {
            new SSResultProperty{Name = nameof(ProcessName), Value = ProcessName },
            new SSResultProperty{Name = nameof(ProcessPath), Value = ProcessPath },
            new SSResultProperty{Name = "PID", Value = ProcessId },
            new SSResultProperty{Name = nameof(SessionId), Value = SessionId }
        };
    }
}

