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
        private OpenAIAPI Api;
        private Conversation chat;
        private TaskCompletionSource taskCompletionSource; 
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
            //如果用户没有输入文本，则不发送
            if(string.IsNullOrEmpty(UserText))
            {
                return;
            }
            //如果上一次的对话还没有结束，则不发送
            if (taskCompletionSource != null && !taskCompletionSource.Task.IsCompleted)
            {
                return;
            }
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
                taskCompletionSource = new TaskCompletionSource();
                await foreach (var txt in chat.StreamResponseEnumerableFromChatbotAsync())
                {
                    if (AiRespondModel.Text == "Thinking...") 
                    { 
                        AiRespondModel.Text = string.Empty; 
                    }
                    AiRespondModel.Text += txt;
                }
                taskCompletionSource.SetResult();
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
