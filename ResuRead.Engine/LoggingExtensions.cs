﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ResuRead.Engine;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Add Serilog as the log writer.
        /// </summary>
        /// <param name="services">Service provider to add logger to.</param>
        /// <param name="config">Configuration to be provided to logger.</param>
        public static void AddLogging(this IServiceCollection services, IConfiguration config)
        {
            IConfigurationSection loggingConfig = config.GetSection(Strings.LOGGINGELEMENT);

            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console();

            if (loggingConfig != null && !string.IsNullOrWhiteSpace(loggingConfig[Strings.LOGGING_FILEPATH])) {
                loggerConfig.WriteTo.File(loggingConfig[Strings.LOGGING_FILEPATH], rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7);
            }
            using var logger = loggerConfig.CreateLogger();

            services.AddSingleton<Serilog.ILogger>(logger);
        }
    }
}