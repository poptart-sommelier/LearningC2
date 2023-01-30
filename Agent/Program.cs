using Agent.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    internal class Program
    {
        private static AgentMetadata _metadata;
        
        static void Main(string[] args)
        {
            GenerateMetadata();
        }

        static void GenerateMetadata()
        {
            var process = Process.GetCurrentProcess();

            _metadata = new AgentMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Hostname = Environment.MachineName,
                Username = Environment.UserName,
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                Integrity = TokenInformation.TokenInfo.GetProcessIntegrityLevel(process),
                Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86"
            };

            Console.WriteLine("");
        }
    }
}
