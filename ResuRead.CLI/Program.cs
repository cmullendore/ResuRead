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
        }
    }
}
