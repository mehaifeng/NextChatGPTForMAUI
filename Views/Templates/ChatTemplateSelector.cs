using NextChatGPTForMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Views.Templates
{
    internal class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserChatTemplate { get; set; }
        public DataTemplate AiChatTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(item is ChatModel chatMessage)
            {
                return chatMessage.IsUser ? UserChatTemplate : AiChatTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
