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
using Newtonsoft.Json;
using CommunityToolkit.Mvvm.Messaging;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class ChatPageViewModel:ObservableObject
    {
        private readonly string path = FileSystem.Current.AppDataDirectory + "/parameter.json";
        private ParameterModel paraConfig;
        private OpenAIAPI api;
        private Conversation chat;
        private IList<ChatMessage> messages;
        private TaskCompletionSource taskCompletionSource; 

        #region 构造函数
        public ChatPageViewModel()
        {
            WeakReferenceMessenger.Default.Register<WeakReferenceMessenger, string>(this, "ParameterConfigSetup", (r, m) =>
            {
                ChatPageInitial();
            });
            messages = new List<ChatMessage>();
            ChatPageInitial();
        }
        #endregion

        #region 函数
        /// <summary>
        /// 初始化配置/重新读取配置
        /// </summary>
        private void ChatPageInitial()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                paraConfig = JsonConvert.DeserializeObject<ParameterModel>(json);
                api = new OpenAIAPI(paraConfig.Apikey);
                chat = api.Chat.CreateConversation(new ChatRequest()
                {
                    Model = string.IsNullOrEmpty(paraConfig.Model)? 
                    "gpt-3.5-turbo" : paraConfig.Model,

                    Temperature = string.IsNullOrEmpty(paraConfig.Temperature) ? 
                    1 : Convert.ToDouble(paraConfig.Temperature),

                    TopP = string.IsNullOrEmpty(paraConfig.Top_p) ? 
                    1 : Convert.ToDouble(paraConfig.Top_p),

                    FrequencyPenalty = string.IsNullOrEmpty(paraConfig.Frequency_penalty)? 
                    0 : Convert.ToDouble(paraConfig.Frequency_penalty),

                    PresencePenalty = string.IsNullOrEmpty(paraConfig.Presence_penalty) ? 
                    0 : Convert.ToDouble(paraConfig.Presence_penalty),

                    MaxTokens = string.IsNullOrEmpty(paraConfig.Max_tokens)? 
                    2000 : Convert.ToInt32(paraConfig.Max_tokens),

                    Messages = messages
                });
            }
            else
            {
                api = new OpenAIAPI("");
                chat = api.Chat.CreateConversation(new ChatRequest()
                {
                    Model = "gpt-3.5-turbo",
                });
            }
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
            messages.Add(new ChatMessage
            {
                Content = AiRespondModel.Text,
                Name = "assistant"
            });
        }
        #endregion

        #region 命令
        /// <summary>
        /// 发送
        /// </summary>
        [RelayCommand]
        public async void Send()
        {
            //如果用户没有输入文本，则不发送
            if(string.IsNullOrEmpty(UserText))
            {
                return;
            }
            if(paraConfig==null||string.IsNullOrEmpty(paraConfig.Apikey))
            {
                //goto到设置页面
                await Shell.Current.GoToAsync("ParameterPage");
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
            messages.Add(new ChatMessage
            {
                 Content = UserText,
                 Name = "user"  
            });
            UserText = string.Empty;
            WillShowResultFromAPI();
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
