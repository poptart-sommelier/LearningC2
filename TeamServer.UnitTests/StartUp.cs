using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TeamServer.Services;

namespace TeamServer.UnitTests
{
    public class StartUp
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        public void configureServices(IServiceCollection services)
        {
            builder.Services.AddSingleton<IListenerService, ListenerService>();
            builder.Services.AddSingleton<IAgentService, AgentService>();
        }
    }
}
