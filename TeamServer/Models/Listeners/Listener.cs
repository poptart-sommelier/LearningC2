using TeamServer.Services;

namespace TeamServer.Models.Listeners
{
    public abstract class Listener
    {
        public abstract string Name { get; set; }

        protected IAgentService AgentService;

        public void Init(IAgentService agentService)
        {
            AgentService = agentService;
        }

        public abstract Task Start();
        public abstract void Stop();
    }
}
