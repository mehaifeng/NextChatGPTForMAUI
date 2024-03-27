using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextChatGPTForMAUI.Models
{
    public class ChatCompletionStream
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }

        public class Choice
        {
            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("delta")]
            public Delta Delta { get; set; }

            [JsonProperty("logprobs")]
            public object Logprobs { get; set; }

            [JsonProperty("finish_reason")]
            public string FinishReason { get; set; }
        }

        public class Delta
        {
            [JsonProperty("role")]
            public string Role { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }
        }
    }
}
