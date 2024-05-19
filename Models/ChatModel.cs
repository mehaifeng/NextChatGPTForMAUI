using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextChatGPTForMAUI.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public partial class ChatModel:ObservableObject
    {
        public ChatModel()
        {
        }
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
        private string aiFace;
        public string AIFace
        {
            get { return aiFace; }
            set { SetProperty(ref aiFace, value); }
        }

        private string userFace;
        public string UserFace
        {
            get { return userFace; }
            set { SetProperty(ref userFace, value); }
        }

        private bool messageMenuState = false;
        public bool MessageMenuState
        {
            get { return messageMenuState; }
            set { SetProperty(ref messageMenuState, value); }
        }
    }
}
