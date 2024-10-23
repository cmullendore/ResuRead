using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ResuRead.Engine;
using Serilog;
using System.Text.Json;

namespace ResuRead.Models.OpenAI
{
    public class ChatGPT : AgentModelBase
    {
        private readonly OpenAIClient client;

        private readonly string? _modelName;

        public ChatGPT(ILogger logger, IConfiguration configuration) : base(logger, configuration) 
        {
            //_logger.ForContext<ChatGPT>();

            //client = new OAI.OpenAIClient(_configuration["ApiKey"]);

            //_modelName = _configuration["ModelName"];

            HttpClient httpClient = new HttpClient();

            client = new OpenAIClient(httpClient);
        }

        public override ResumeResponse GetResponse(ResumeRequest resumeContent)
        {
        //    ResumeResponse resumeResponse = new ResumeResponse()
        //    {
        //        CandidateName = "Candidate Name",
        //        ContactEmail = "candidate_email@domain.com",
        //        ContactPhone = "(123) 123-1234",

        //    };

        //    resumeResponse.WorkHistory.Add(new WorkHistoryItem()
        //    {
        //        CompanyName = "Company Name",
        //        RoleTitle = "Role Title",
        //        StartDate = DateOnly.Parse("1/1/2024"),
        //        EndDate = DateOnly.Parse("12 / 1 / 2024")
        //    });

        //    JsonSerializerOptions options = new JsonSerializerOptions();
        //    options.WriteIndented = true;
        //    options.IncludeFields = true;

        //    _logger.Debug(JsonSerializer.Serialize(resumeResponse, options));

            return new ResumeResponse();
        }
        
        public override async Task InitializeAsync(string prompt)
        {
            //_logger.Debug($"Initializing agent model {_modelName} with initialization prompt {prompt}.");

            //OAICHAT.ChatClient chatClient = client.GetChatClient(_modelName);

            //var chatCompletion = await chatClient.CompleteChatAsync(prompt);
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
