using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public class ChatRequest
    {
        public string model { get; set; }
        public List<ChatMessage> messages { get; set; }
        public double? temperature { get; set; }
        public double? top_p { get; set; }
        public int? n { get; set; }
        public bool? stream { get; set; }
        public List<string> stop { get; set; }
        public int? max_tokens { get; set; }
        public double? presence_penalty { get; set; }
        public double? frequency_penalty { get; set; }
        public Dictionary<string, object> logit_bias { get; set; }
        public string user { get; set; }
    }
}
