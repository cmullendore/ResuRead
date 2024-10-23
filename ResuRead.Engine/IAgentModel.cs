using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResuRead.Engine
{
    /// <summary>
    /// Universal interface for primary interactions with an LLM/AI model.
    /// </summary>
    public interface IAgentModel
    {
        /// <summary>
        /// Initialize the model with objective and bounding prompts.
        /// </summary>
        /// <param name="prompt">The text of the prompt to be submitted to the model.</param>
        /// <returns></returns>
        public Task InitializeAsync(string prompt);

        /// <summary>
        /// Submit the resume content to the configured provider for processing and population of the ResumeResponse.
        /// </summary>
        /// <param name="resumeContent">The configured ResumeRequest containing either the resume text or the path to a resume file.</param>
        /// <returns>Structured populated response from the model based on the submitted resume.</returns>
        public ResumeResponse GetResponse(ResumeRequest resumeContent);

        /// <summary>
        /// Reset the model in preparation for the next request.
        /// </summary>
        /// <param name="prompt">Prompt to reset the model. If null, fully closes the model causing a new instance of the model to be created with each request.</param>
        public void Reset(string? prompt);

    }
}
