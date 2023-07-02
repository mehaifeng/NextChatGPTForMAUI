using NextChatGPTForMAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API.Models;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class ChatPageViewModel:ObservableObject
    {
        OpenAIAPI Api;
        Conversation chat;
        #region 构造函数
        public ChatPageViewModel()
        {
            Api = new OpenAIAPI(new APIAuthentication("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", "org-lArMg7f1I8srwSviWaL7MuXm"));
            chat = Api.Chat.CreateConversation(new ChatRequest()
            {
                Model = "gpt-3.5-turbo-16k"
            }) ;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 发送
        /// </summary>
        [RelayCommand]
        public void Send()
        {
            ChatList.Add(new ChatModel
            {
                IsUser = true,
                Text = UserText
            }) ;
            chat.AppendUserInput(new string(UserText));
            UserText = string.Empty;
            WillShowResultFromAPI();
        }
        /// <summary>
        /// 显示回复
        /// </summary>
        private void WillShowResultFromAPI()
        {
            ChatModel AiRespondModel = new()
            {
                Text = "Thinking...",
                IsUser = false
            };
            Thread thread = new (async () =>
            {
                await foreach (var txt in chat.StreamResponseEnumerableFromChatbotAsync())
                {
                    if (AiRespondModel.Text == "Thinking...") 
                    { 
                        AiRespondModel.Text = string.Empty; 
                    }
                    AiRespondModel.Text += txt;
                }
            });
            thread.Start();
            ChatList.Add(AiRespondModel);
        }
        #endregion

        #region 属性
        /// <summary>
        /// AI和用户的所有对话框信息集合
        /// </summary>
        [ObservableProperty]
        ObservableCollection<ChatModel> chatList = new();
        /// <summary>
        /// 用户输入的文本
        /// </summary>
        [ObservableProperty]
        string userText;
        #endregion
    }
}
