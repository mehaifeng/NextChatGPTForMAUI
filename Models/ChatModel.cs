using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public partial class ChatModel:ObservableObject
    {
        private string tag;
        public string Tag
        {
            get { return tag; }
            set { SetProperty(ref tag, value); }
        }

        private string text;
        public string Text
        {
            get { return text; } 
            set
            {
                SetProperty(ref text, value);
            }
        }

        private bool isUser;
        public bool IsUser
        {
            get { return isUser; }
            set { SetProperty(ref isUser, value); }
        }
    }
}
