using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ResuRead.Engine;
using System.Text.Json;

namespace ResuRead.CLI
{
    internal class Program
    {
        IServiceProvider Services {  get; }

        static void Main(string[] args)
        {
            string? resumePath = string.Empty;

            while (args.Length == 0 && string.IsNullOrWhiteSpace(resumePath) && !File.Exists(resumePath))
            {
                Console.WriteLine("Please provide the full path to the resume to be analyzed and press enter:");
                resumePath = Console.ReadLine();

                if (!File.Exists(resumePath))
                {
                    Console.WriteLine("A file could not be located on that path. Please try again.");
                }

            }

            if (args.Length > 0) {
                 resumePath = args[0];
            }

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.Sources.Clear();

            builder.Configuration.AddEnvironmentVariables();

            builder.Configuration.AddJsonFile(Strings.CONFIGFILENAME);

            string? secretsKey = builder.Configuration.GetValue<string>("SecretsKey");

            if (!string.IsNullOrWhiteSpace(secretsKey))
            {
                builder.Configuration.AddUserSecrets(secretsKey);
            }

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

            IAgentModel model = modelFactory.CreateAgentModel().Result;

            ResumeRequest resumeRequest = new ResumeRequest()
            {
                ResumeFilePath = resumePath
            };

            var resp = model.GetResponseAsync(resumeRequest).Result;

            log.Information($"Completed! JSON result:\n");

            log.Information(JsonSerializer.Serialize(resp, new JsonSerializerOptions() { WriteIndented = true}));

            Console.WriteLine("Conversion Complete. Press any key to exit.");

            Console.ReadKey();
        }
    }
}
