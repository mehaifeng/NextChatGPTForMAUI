using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public class HistoryChatRequest
    {
        /// <summary>
        /// 历史消息唯一标识符时间
        /// </summary>
        public string TimeId { get; set; }
        /// <summary>
        /// 历史对话标题
        /// </summary>
        public string HistoryTitle { get; set; }
        /// <summary>
        /// 历史对话
        /// </summary>
        public ChatRequest History { get; set; }
        /// <summary>
        /// 历史对话双方头像
        /// </summary>
        public List<string> BothAvatars { get; set; }
        /// <summary>
        /// 历史对话结构
        /// </summary>
        public List<ChatModel> HistoryChatModel { get; set; }
    }
}
