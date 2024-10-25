using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ResuRead.Engine;
using Serilog;
using System.Text.Json;
using OAI = OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using OpenAI.VectorStores;

#pragma warning disable OPENAI001
namespace ResuRead.Models.OpenAI
{
    public class ChatGPT : AgentModelBase
    {
        private readonly string? _modelName;

        private readonly OpenAIClient openAIClient;
        private readonly OpenAIFileClient fileClient;
        private readonly AssistantClient assistantClient;

        private string initializationPrompt = string.Empty;

        public ChatGPT(ILogger logger, IConfiguration configuration) : base(logger, configuration) 
        {
            _modelName = _configuration["ModelName"];

            openAIClient = new(_configuration["ApiKey"]);
            fileClient = openAIClient.GetOpenAIFileClient();
            assistantClient = openAIClient.GetAssistantClient();
            
        }

        /// <summary>
        /// Accept the provided resume file path, upload, pass to the Assistant, receive the response, and remove the uploaded file.
        /// </summary>
        /// <param name="resumeContent">The ResumeContent object containing the ResumeFilePath of the resume to be uploaded.
        /// This implementation ignores the ResumeText element. Only file paths will be accepted.</param>
        /// <returns></returns>
        public override async Task<ResumeResponse> GetResponseAsync(ResumeRequest resumeContent)
        {
            // Ensure we have a proper file path.
            // TODO: Limit to specific file types.
            if (resumeContent == null || String.IsNullOrWhiteSpace(resumeContent.ResumeFilePath)) {
                _logger.Error("The full path to the target resume file is required but missing.");
            }
            else if (!File.Exists(resumeContent.ResumeFilePath)) {

                _logger.Error($"Could not locate file {resumeContent.ResumeFilePath}.");
            }

            OpenAIFile resumeUpload = await fileClient.UploadFileAsync(resumeContent.ResumeFilePath, FileUploadPurpose.Assistants);

            VectorStoreClient vectorClient = openAIClient.GetVectorStoreClient();

            var vectorStore = await vectorClient.CreateVectorStoreAsync(true);

            _logger.Debug($"Created vector store id {vectorStore.VectorStoreId}");

            var addVectorFileResult = vectorClient.AddFileToVectorStore(vectorStore.VectorStoreId, resumeUpload.Id, true);

            await addVectorFileResult.WaitForCompletionAsync();

            var createOptions = new AssistantCreationOptions()
            {
                Name = "ResuRead",
                Instructions = initializationPrompt,

            };
            createOptions.Tools.Add(ToolDefinition.CreateFileSearch());
            createOptions.ToolResources = new ToolResources();
            createOptions.ToolResources.FileSearch = new FileSearchToolResources();
            createOptions.ToolResources.FileSearch.VectorStoreIds.Add(vectorStore.VectorStoreId);
            

            Assistant assistant = await assistantClient.CreateAssistantAsync(_modelName, createOptions);
            
            AssistantThread thread = await assistantClient.CreateThreadAsync();


            ClientResult<ThreadRun> run = await assistantClient.CreateRunAsync(thread.Id, assistant.Id);

            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                run = assistantClient.GetRun(thread.Id, run.Value.Id);
            } while (!run.Value.Status.IsTerminal);

            var messages = assistantClient.GetMessages(thread.Id);

            foreach (var message in messages)
            {
                foreach (var text in message.Content)
                {
                    _logger.Debug(text.Text);
                }
            }

            return new ResumeResponse();
        }
        
        public override async Task InitializeAsync(string prompt)
        {
            initializationPrompt = prompt;
        }

        public override void Reset(string? prompt)
        {
            // Do nothing
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
