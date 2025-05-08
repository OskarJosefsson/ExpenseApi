using ExpenseApi.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ExpenseApi.Services
{
    public interface IChatGippityService
    {
        Task<ChatGptReceipt> AnalyzeReceiptImage(string image);
    }

    public class ChatGippityService : IChatGippityService
    {

        private readonly string _key;

        public ChatGippityService(IConfiguration configuration)
        {
            _key = configuration["ChatGpt:Key"];
        }

        public async Task<ChatGptReceipt> AnalyzeReceiptImage(string imageText)
        {
            var prompt = GetPrompt("ReceiptPrompt.txt");
            prompt = LoadPrompt(prompt, imageText, "{imageText}");

            var response = await SendRequestAsync(prompt);

            var choice = response.Choices?.FirstOrDefault();
            if (choice == null || choice.Message == null)
                throw new Exception("No valid message from GPT.");

            var receipt = JsonConvert.DeserializeObject<ChatGptReceipt>(choice.Message.Content);

            return receipt;
        }

        private string LoadPrompt(string prompt, string imageText, string item)
        {
            return prompt.Replace(item, imageText);
        }

        private string GetPrompt(string type)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", type);
            return File.ReadAllText(filePath);
        }

        private async Task<ChatCompletionResponse> SendRequestAsync(string userMessage)
        {
            using var client = new HttpClient();

            var apiKey = _key;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new
            {
                role = "user",
                content = userMessage
            }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Request failed: {response.StatusCode}, {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseString);
            return responseBody;
        }

    }
}
