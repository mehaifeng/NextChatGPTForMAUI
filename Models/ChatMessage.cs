using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextChatGPTForMAUI.Models
{
    public class ChatMessage
    {
        public string role { get; set; }
        public string content { get; set; }
        [JsonIgnore]
        public bool IsMaskType { get; set; }
    }
}
