using Microsoft.Extensions.Configuration;
using Serilog;
using ResuRead.Engine;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NodelFactoryExtensions
    {
        /// <summary>
        /// Add Serilog as the log writer.
        /// </summary>
        /// <param name="services">Service provider to add logger to.</param>
        /// <param name="config">Configuration to be provided to logger.</param>
        public static void AddModelFactory(this IServiceCollection services)
        {
            services.AddSingleton<IModelFactory, ModelFactory>();
        }
    }
}
