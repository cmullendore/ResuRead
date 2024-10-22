using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ResuRead.Engine;
using Serilog;

namespace ResuRead.Models.OpenAI
{
    public class ChatGPT : AgentModelBase
    {
        public ChatGPT(ILogger logger, IConfiguration configuration) : base(logger, configuration) 
        {
            _logger.ForContext<ChatGPT>();
        }

        public override ResumeResponse GetResponse(ResumeRequest resumeContent)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(string modelName, string prompt)
        {
            _logger.Debug("Initializing agent model.");
        }

        public override void Reset(string? prompt)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
