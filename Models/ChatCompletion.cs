using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public class ChatCompletion
    {
        public List<Choice> choices { get; set; }
        public int created { get; set; }
        public string id { get; set; }
        public string model { get; set; }
        public string @object { get; set; }
        public Usage usage { get; set; }

        public class Choice
        {
            public string finish_reason { get; set; }
            public int index { get; set; }
            public ChatMessage message { get; set; }
            public object logprobs { get; set; }
        }
        public class Usage
        {
            public int completion_tokens { get; set; }
            public int prompt_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }
}
