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
        public IAgentModel CreateAgentModel();

    }
}
