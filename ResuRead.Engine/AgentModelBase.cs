using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResuRead.Engine
{
    public abstract class AgentModelBase : IAgentModel, IDisposable
    {
        protected readonly IConfiguration _configuration;

        protected readonly ILogger _logger;

        public AgentModelBase(ILogger logger, IConfiguration configuration)
        {
            _configuration = configuration;

            _logger = logger;
        }

        public abstract Task InitializeAsync(string prompt);

        public abstract Task<ResumeResponse> GetResponseAsync(ResumeRequest resumeContent);

        public abstract void Reset(string? prompt);

        public abstract void Dispose();
    }
}
