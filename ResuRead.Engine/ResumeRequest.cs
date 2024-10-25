using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResuRead.Engine
{
    /// <summary>
    /// Contains the information to be uploaded to the model, either raw text or a file upload via full path.
    /// </summary>
    public class ResumeRequest
    {
        /// <summary>
        /// Simple text-based resume content to be consumed by the agent.
        /// </summary>
        public string? ResumeText { get; set; }

        /// <summary>
        /// Fully qualified path to a resume file to be uploaded and consumed by the agent.
        /// </summary>
        public string? ResumeFilePath { get; set; }
    }
}
