using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ResuRead.Engine;

namespace ResuRead.CLI
{
    internal class Program
    {
        IServiceProvider Services {  get; }

        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.Sources.Clear();

            builder.Configuration.AddJsonFile(Strings.CONFIGFILENAME);

            builder.Configuration.GetSection(Strings.LOGGINGELEMENT);

            builder.Services.AddLogging(builder.Configuration.GetSection(Strings.LOGGINGELEMENT));

            builder.Services.AddModelFactory();

            Log.Debug("Building host");

            var host = builder.Build();

            ILogger log = host.Services.GetRequiredService<ILogger>();

            log.Debug("Builder created.");

            log.Debug("Retrieving ModelFactory.");

            IModelFactory modelFactory = host.Services.GetRequiredService<IModelFactory>();

            log.Debug("Creating instance of model.");

            IAgentModel model = modelFactory.CreateAgentModel();
        }
    }
}
