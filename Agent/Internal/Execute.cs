using System.Diagnostics;
using System.IO;

namespace Agent.Internal
{
    public static class Execute
    {
        public static string ExecuteCommand(string fileName, string arguments)
        {
            //TODO: spoof commandline args here
            //TODO: spoof parent process here
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            string output = "";

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.OutputDataReceived += (_, e) => { output += e.Data; };
            process.ErrorDataReceived += (_, e) => { output += e.Data; };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            return output;
        }
    }
}
