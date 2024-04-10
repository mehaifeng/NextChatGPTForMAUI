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
            AsyncRelayCommand ChangeChatFaceCommand = new AsyncRelayCommand(ChangeChatFace);
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
        private bool chatFace;
        public bool ChatFace
        {
            get { return chatFace; }
            set { SetProperty(ref chatFace, value); }
        }
        public AsyncRelayCommand ChangeChatFaceCommand;
        private Task ChangeChatFace()
        {
            ChatFacePopup chatFacePopup = new ChatFacePopup();
            Application.Current.MainPage.ShowPopupAsync(chatFacePopup);
            return Task.CompletedTask;
        }
    }
}
