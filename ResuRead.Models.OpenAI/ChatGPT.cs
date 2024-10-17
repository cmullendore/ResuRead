using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResuRead.Engine;

namespace ResuRead.Models.OpenAI
{
    public class ChatGPT : IAgentModel
    {
        public ResumeResponse GetResponse(ResumeRequest resumeContent)
        {
            throw new NotImplementedException();
        }

        public string Initialize(string modelName, string prompt)
        {
            throw new NotImplementedException();
        }

        public void Reset(string? prompt)
        {
            throw new NotImplementedException();
        }
    }
}
