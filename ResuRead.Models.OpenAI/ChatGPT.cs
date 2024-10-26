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
using System.Net.Http.Headers;

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
        public override async Task<ResumeResponse?> GetResponseAsync(ResumeRequest resumeContent)
        {
            // Ensure we have a proper file path.
            // TODO: Limit to specific file types.
            if (resumeContent == null || String.IsNullOrWhiteSpace(resumeContent.ResumeFilePath)) {
                _logger.Error("The full path to the target resume file is required but missing.");
            }
            else if (!File.Exists(resumeContent.ResumeFilePath)) {

                _logger.Error($"Could not locate file {resumeContent.ResumeFilePath}.");
            }

            _logger.Information("Uploading file to ChatGPT");

            OpenAIFile resumeUpload = await fileClient.UploadFileAsync(resumeContent.ResumeFilePath, FileUploadPurpose.Assistants);

            while (resumeUpload.Status == FileStatus.Uploaded)
            {
                _logger.Information("Waiting for uploaded file to be accepted...");

                Thread.Sleep(1000);
                resumeUpload = await fileClient.GetFileAsync(resumeUpload.Id);
            }

            VectorStoreClient vectorClient = openAIClient.GetVectorStoreClient();

            var vectorStore = await vectorClient.CreateVectorStoreAsync(true);

            _logger.Debug($"Created vector store id {vectorStore.VectorStoreId}");

            var addVectorFileResult = await vectorClient.AddFileToVectorStoreAsync(vectorStore.VectorStoreId, resumeUpload.Id, true);

            while (!addVectorFileResult.Status.HasValue || addVectorFileResult.Status.Value == VectorStoreFileAssociationStatus.InProgress)
            {
                _logger.Information("Waiting for uploaded file to be processed...");

                Thread.Sleep(1000);
                await addVectorFileResult.UpdateStatusAsync();
            }

            var createOptions = new AssistantCreationOptions()
            {
                Name = "ResuRead",
                Instructions = initializationPrompt,
                Tools =
                {
                    new FileSearchToolDefinition()
                },
                ToolResources = new ToolResources()
                {
                    FileSearch = new FileSearchToolResources()
                    {
                        VectorStoreIds =
                        {
                            vectorStore.VectorStoreId
                        }
                    }
                }
            };

            Assistant assistant = await assistantClient.CreateAssistantAsync(_modelName, createOptions);
            
            AssistantThread thread = await assistantClient.CreateThreadAsync(new ThreadCreationOptions()
            {
                 InitialMessages = {
                    new ThreadInitializationMessage(MessageRole.User, [
                            initializationPrompt
                        ])
                 }, 
                ToolResources = new ToolResources()
                {
                     FileSearch = new()
                     {
                          VectorStoreIds = {
                             vectorStore.VectorStoreId
                         }
                     }
                }
            });

            _logger.Information("Analyzing file.");

            ClientResult<ThreadRun> run = await assistantClient.CreateRunAsync(thread.Id, assistant.Id);

            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                run = assistantClient.GetRun(thread.Id, run.Value.Id);
            }
            while (!run.Value.Status.IsTerminal);

            _logger.Information("Retrieving result.");

            var messages = assistantClient.GetMessages(thread.Id);

            ThreadMessage? lastMessage = messages.OrderBy(m => m.CreatedAt).LastOrDefault();

            if (lastMessage == null)
            {
                _logger.Error("No response was received from ChatGPT.");

                return null;
            }


            _logger.Information("Performing cleanup.");

            _ = await fileClient.DeleteFileAsync(resumeUpload.Id);
            _ = await vectorClient.DeleteVectorStoreAsync(vectorStore.VectorStoreId);
            _ = await assistantClient.DeleteAssistantAsync(assistant.Id);

            _logger.Information("Parsing result into native object for return.");

            string result = lastMessage.Content[0].Text;

            result = result.Replace("```json", null).Replace("```", null);

            ResumeResponse? response = null;

            try
            {
                response = JsonSerializer.Deserialize(result, typeof(ResumeResponse)) as ResumeResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to parse JSON response: {ex.Message}");
            }

            return response;
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
