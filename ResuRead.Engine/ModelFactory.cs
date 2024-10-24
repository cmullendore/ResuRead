using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

namespace ResuRead.Engine
{
    public class ModelFactory : IModelFactory
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger _log;

        private readonly Assembly _modelAssembly;

        private readonly Type? _modelType;

        public ModelFactory(ILogger logger, IConfiguration configuration)
        {
            _configuration = configuration;

            _log = logger.ForContext<ModelFactory>();

            string? assemblyPath = configuration[Strings.AGENTCONFIG_LIBRARYFILENAME];

            if (string.IsNullOrWhiteSpace(assemblyPath)) 
            {
                _log.Error($"{Strings.AGENTCONFIG_LIBRARYFILENAME} not defined in configuration.");
            }

            assemblyPath = Path.GetFullPath(assemblyPath);

            if (!File.Exists(assemblyPath))
            {
                _log.Error($"Library file {assemblyPath} not found.");
            }

            string? className = configuration[Strings.AGENTCONFIG_CLASSNAME];

            if (string.IsNullOrWhiteSpace(className))
            {
                _log.Error($"{Strings.AGENTCONFIG_CLASSNAME} not defined in configuration.");
            }

            _log.Debug($"Locating and loading {className} from {assemblyPath}.");

            try
            {

                _modelAssembly = Assembly.LoadFrom(assemblyPath);

                var files = Directory.GetFiles(Path.GetDirectoryName(assemblyPath), "*.dll");

                foreach (var file in files)
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                }

            }
            catch (Exception ex)
            {
                // Log the error but then re-throw the exception to be handled by the caller.
                _log.Error(ex, $"Error loading model assembly {assemblyPath}: {ex.Message}");
                throw;
            }

            try
            {
                // Each assembly should only implement one model (by convention),
                // so do a search for the first type that implements the interface and use it.
                // This saves us from having to know the class name... It will be the only one
                // that implements the interface in the assembly anyway.
                _modelType = _modelAssembly.GetType(className);
            }
            catch (Exception ex)
            {
                // Log the error but then re-throw the exception to be handled by the caller.
                _log.Error(ex, $"Error while searching for IAgentModel type: {ex.Message}");
                throw;
            }

            if (_modelType == null)
            {
                _log.Error("Could not locate a type implementing the IAgentModel interface in assembly.");
            }
            

        }

        public async Task<IAgentModel> CreateAgentModel()
        {
            //AssemblyDependencyResolver resolver = new AssemblyDependencyResolver(_configuration[Strings.AGENTCONFIG_LIBRARYFILENAME]);

            object[] args = { _log, _configuration.GetSection(Strings.AGENTCONFIG_PARAMETERS) };

            IAgentModel? model = _modelAssembly.CreateInstance(_modelType.FullName, true, BindingFlags.Default, null, args, null, null) as IAgentModel; //Activator.CreateInstance(_modelType, _log, _configuration.GetSection(Strings.AGENTCONFIG_PARAMETERS)) as IAgentModel;

            if (null == model)
            {
                _log.Error($"Failed to create an instance of model {_modelType.Name}");

                throw new NullReferenceException($"Failed to create an instance of model {_modelType.Name}");
            }

            await model.InitializeAsync(_configuration[Strings.PROMPT_INITIALIZATION]);

            return model;
        }
    }
}
