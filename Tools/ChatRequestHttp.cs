using NextChatGPTForMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using CommunityToolkit.Mvvm;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NextChatGPTForMAUI.Tools
{
    public class ChatRequestHttp:ObservableObject
    {
        private static readonly string apiurl = "https://api.nextapi.fun/v1/chat/completions";

        public static async IAsyncEnumerable<string> ReceiveStreamResponseFromOpenAI(ChatRequest chatRequest,string api_address,string apikey)
        {
            api_address ??= "https://api.nextapi.fun/";
            Uri baseUri = new Uri(api_address);
            Uri relativeUri = new Uri("v1/chat/completions", UriKind.Relative);
            Uri combinedUri = new Uri(baseUri, relativeUri);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apikey);
                var requestBody = JsonConvert.SerializeObject(chatRequest);
                var requestContent = new StringContent(requestBody, Encoding.UTF8,"application/json");
                HttpResponseMessage response = await client.PostAsync(combinedUri, requestContent);
                bool isFinish = false;
                if(response.IsSuccessStatusCode)
                {
                    using(Stream stream = await response.Content.ReadAsStreamAsync())
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (line.StartsWith("data: "))
                            { 
                                string chunk = line[6..];
                                if (!string.IsNullOrEmpty(chunk))
                                {
                                    
                                    ChatCompletionStream chatCompletion = JsonConvert.DeserializeObject<ChatCompletionStream>(chunk);
                                    if (!string.IsNullOrEmpty(chatCompletion.Choices[0].Delta.Content))
                                    {
                                        string text = chatCompletion.Choices[0].Delta.Content;
                                        yield return text;
                                    }
                                    if (chatCompletion.Choices[0].FinishReason == "stop")
                                    {
                                        isFinish = true;
                                        yield break;
                                    }
                                }
                            }
                        }
                        if (!isFinish)
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    yield return "Error: " + response.StatusCode;
                }
            }
        }
    }
}
