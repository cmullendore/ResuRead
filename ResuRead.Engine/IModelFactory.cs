using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResuRead.Engine
{
    public interface IModelFactory
    {
        /// <summary>
        /// Create an instance of the configured agent model to be used.
        /// </summary>
        /// <returns>An instance of the configured agent.</returns>
        public Task<IAgentModel> CreateAgentModel();

    }
}
