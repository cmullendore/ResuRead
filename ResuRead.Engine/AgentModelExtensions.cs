using Microsoft.Extensions.Configuration;
using Serilog;
using ResuRead.Engine;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AgentModelExtensions
    {
        /// <summary>
        /// Add Serilog as the log writer.
        /// </summary>
        /// <param name="services">Service provider to add logger to.</param>
        /// <param name="config">Configuration to be provided to logger.</param>
        public static void AddAgentModel(this IServiceCollection services,ILogger logger, IConfiguration config)
        {

            string? assemblyPath = config[Strings.AGENTCONFIG_LIBRARYFILENAME];

            if (string.IsNullOrWhiteSpace(assemblyPath))
            {
                logger.Error($"{Strings.AGENTCONFIG_LIBRARYFILENAME} not defined in configuration.");
            }

            if (!File.Exists(assemblyPath))
            {
                logger.Error($"Library file {assemblyPath} not found.");
            }

            string? className = config[Strings.AGENTCONFIG_CLASSNAME];

            if (string.IsNullOrWhiteSpace(className))
            {
                logger.Error($"{Strings.AGENTCONFIG_CLASSNAME} not defined in configuration.");
            }

              logger.Debug($"Locating and loading {className} from {assemblyPath}.");

            Assembly _modelAssembly;

            try
            {
                _modelAssembly = Assembly.Load(assemblyPath);
            }
            catch (Exception ex)
            {
                // Log the error but then re-throw the exception to be handled by the caller.
                logger.Error(ex, $"Error loading model assembly {assemblyPath}: {ex.Message}");
                throw;
            }

            Type modelType;

            try
            {
                // Each assembly should only implement one model (by convention),
                // so do a search for the first type that implements the interface and use it.
                // This saves us from having to know the class name... It will be the only one
                // that implements the interface in the assembly anyway.
                modelType = _modelAssembly.ExportedTypes
                    .Where(types => types.IsAssignableFrom(typeof(IAgentModel)))
                    .First();
            }
            catch (Exception ex)
            {
                // Log the error but then re-throw the exception to be handled by the caller.
                logger.Error(ex, $"Error while searching for IAgentModel type: {ex.Message}");
                throw;
            }

            if (modelType == null)
            {
                logger.Error("Could not locate a type implementing the IAgentModel interface in assembly.");
            }


        }
    }
}
