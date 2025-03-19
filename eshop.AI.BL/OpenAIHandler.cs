using eshop.AI.BL.Interfaces;
using eshop.AI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eshop.AI.BL
{
    public class OpenAIHandler : IAIHandler
    {
        private string _apiUrl;
        private string _apiKey;

        public OpenAIHandler(string apiUrl, string apiKey)
        {
            _apiUrl = apiUrl;
            _apiKey = apiKey;
        }

        public async Task<string> SendRequestAsync(string model, float temperature, string systemMessage, string userMessage)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var request = new OpenAIRequest()
            {
                model = model,
                temperature = temperature,
                messages = new List<OpenAIRequestMessage>()
                {
                    new OpenAIRequestMessage() { role = "system", content = systemMessage },
                    new OpenAIRequestMessage() { role = "user", content = userMessage }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(_apiUrl, content).ConfigureAwait(false);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                return "Error while generating response";
            }

            var openAIResponse = JsonConvert.DeserializeObject<OpenAIChatCompletionResponse>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false)
            );

            return openAIResponse.Choices[0].Message.Content;
        }
    }
}
