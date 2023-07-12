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
using System.Security.Authentication;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using NextChatGPTForMAUI.Views;
using NextChatGPTForMAUI.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class ChatPageViewModel:ObservableObject
    {
        #region 本地私有属性
        private readonly string path = $"{FileSystem.Current.AppDataDirectory}/parameter.json";
        private readonly string savePath = $"{FileSystem.Current.AppDataDirectory}/saveFile.json";
        private List<HistoryChatRequest> historyChatRequestsList;
        private ParameterModel paraConfig;
        private OpenAIAPI api;
        private ChatRequest chatRequest;
        private TaskCompletionSource taskCompletionSource;
        private string oldChatTimeId;
        #endregion

        #region 构造函数
        public ChatPageViewModel()
        {
            //载入聊天配置
            WeakReferenceMessenger.Default.Register<WeakReferenceMessenger, string>(this, "ParameterConfigSetup", (r, m) =>
            {
                ChatPageInitial();
            });
            //载入历史对话
            WeakReferenceMessenger.Default.Register<HistoryChatRequest, string>(this, "ReloadHistoryChat", (r, m) =>
            {
                oldChatTimeId = m.TimeId;
                ChatList = new ObservableCollection<ChatModel>(m.HistoryChatModel);
                chatRequest = m.History;
            });
            //刷新历史对话列表
            WeakReferenceMessenger.Default.Register<List<HistoryChatRequest>,string>(this,"RefreshHistoryChatList",(r,m)=>
            {
                historyChatRequestsList = m;
            });
            ChatList = new ObservableCollection<ChatModel>();
            historyChatRequestsList = new List<HistoryChatRequest>();
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
                //初始化ChatAPI参数/重新读取配置并加载到ChatAPI参数
                LoadChatApiPara();
            }
            else
            {
                api = new OpenAIAPI("");
            }
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                historyChatRequestsList = JsonConvert.DeserializeObject<List<HistoryChatRequest>>(json);
            }
        }
        /// <summary>
        /// 装载ChatApi参数
        /// </summary>
        private void LoadChatApiPara()
        {
            chatRequest = new ChatRequest()
            {
                Model = string.IsNullOrEmpty(paraConfig.Model) ?
                    "gpt-3.5-turbo" : paraConfig.Model,

                Temperature = string.IsNullOrEmpty(paraConfig.Temperature) ?
                    1 : Convert.ToDouble(paraConfig.Temperature),

                TopP = string.IsNullOrEmpty(paraConfig.Top_p) ?
                    1 : Convert.ToDouble(paraConfig.Top_p),

                FrequencyPenalty = string.IsNullOrEmpty(paraConfig.Frequency_penalty) ?
                    0 : Convert.ToDouble(paraConfig.Frequency_penalty),

                PresencePenalty = string.IsNullOrEmpty(paraConfig.Presence_penalty) ?
                    0 : Convert.ToDouble(paraConfig.Presence_penalty),

                MaxTokens = string.IsNullOrEmpty(paraConfig.Max_tokens) ?
                    2000 : Convert.ToInt32(paraConfig.Max_tokens),

                Messages = new List<ChatMessage>()
            };
        }

        /// <summary>
        /// 显示回复
        /// </summary>
        private async void WillShowResultFromAPI(Border o)
        {
            bool isCorrect = true;
            taskCompletionSource = new TaskCompletionSource();
            ChatModel AiRespondModel = new()
            {
                Text = "Thinking...",
                IsUser = false
            };
            ChatList.Add(AiRespondModel);
            Thread thread = new(async () =>
            {
                try
                {
                    await foreach (var txt in api.Chat.StreamChatEnumerableAsync(chatRequest))
                    {
                        if (AiRespondModel.Text == "Thinking...")
                        {
                            AiRespondModel.Text = string.Empty;
                        }
                        AiRespondModel.Text += txt.Choices[0].Delta.Content;
                    }
                    taskCompletionSource.SetResult();
                }
                catch (Exception ex)
                {
                    isCorrect = false;
                    if (ex is AuthenticationException)
                    {
                        if (ChatList.Count >= 2)
                        {
                            ChatList.Remove(ChatList.Last());
                        }
                        static async void action() { await Shell.Current.GoToAsync("//ParameterPage"); }
                        ShowSnackBar("你的ApiKey是无效的，请检查后重试", action, "点击跳转到配置", o);
                        taskCompletionSource.SetResult();
                    }
                    else if(ex is HttpRequestException)
                    {
                        if (ChatList.Count >= 2)
                        {
                            ChatList.Remove(ChatList.Last());
                        }
                        static async void action() { await Shell.Current.GoToAsync("//ParameterPage"); }
                        ShowSnackBar("HttpRequestException错误，请检查配置或网络", action, "点击跳转到配置", o);
                        taskCompletionSource.SetResult();
                    }
                }
            });
            thread.Start();
            await taskCompletionSource.Task;
            if (isCorrect)
            {
                chatRequest.Messages.Add(new ChatMessage
                {
                    Role = ChatMessageRole.Assistant,
                    Content = AiRespondModel.Text
                });
            }
            isCorrect = true;
        }
        /// <summary>
        /// 在选定的控件上方显示Snackbar提示
        /// </summary>
        /// <param name="snackText"></param>
        /// <param name="action"></param>
        /// <param name="buttonText"></param>
        /// <param name="o"></param>
        private static async void ShowSnackBar(string snackText,Action action, string buttonText,Border o)
        {
            SnackbarOptions snackbarOptions = new()
            {
                BackgroundColor = Color.FromArgb("#ce2029"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = 10
            };
            await o.DisplaySnackbar(snackText, action, buttonText, TimeSpan.FromSeconds(5), snackbarOptions);
        }
        #endregion

        #region 命令
        /// <summary>
        /// 发送
        /// </summary>
        [RelayCommand]
        public async Task Send(Border o)
        {
            //如果用户没有输入文本，则不发送
            if(string.IsNullOrEmpty(UserText))
            {
                return;
            }
            if(paraConfig==null||string.IsNullOrEmpty(paraConfig.Apikey))
            {
                //goto到设置页面
                await Shell.Current.GoToAsync("//ParameterPage");
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
            chatRequest.Messages.Add(new ChatMessage
            {
                Role = ChatMessageRole.User,
                Content = UserText
            });
            UserText = string.Empty;
            WillShowResultFromAPI(o);
        }
        /// <summary>
        /// 清空/保存
        /// </summary>
        [RelayCommand]
        public async Task ClearAndSave()
        {
            if (taskCompletionSource == null)
            {
                oldChatTimeId = string.Empty;
                ChatList.Clear();
                chatRequest.Messages.Clear();
            }
            if (ChatList.Count > 0 && taskCompletionSource.Task.IsCompleted)
            {
                if (historyChatRequestsList.Count == 0)
                {
                    oldChatTimeId = string.Empty;
                }
                if(string.IsNullOrEmpty(oldChatTimeId))
                {
                    chatRequest.Messages.Add(new ChatMessage
                    {
                        Role = ChatMessageRole.User,
                        Content = "请根据以上内容总结一句简短的标题，50字以内"
                    });
                    var summary = await api.Chat.CreateChatCompletionAsync(chatRequest);
                    chatRequest.Messages.Remove(chatRequest.Messages.Last());
                    historyChatRequestsList.Add(new HistoryChatRequest
                    {
                        TimeId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        HistoryTitle = summary.ToString(),
                        History = new ChatRequest(chatRequest),
                        HistoryChatModel = ChatList.ToList()
                    }) ;
                }
                else if(historyChatRequestsList.Any(t=>t.TimeId==oldChatTimeId))
                {
                    if(historyChatRequestsList.First(t => t.TimeId == oldChatTimeId).HistoryChatModel.Count != ChatList.Count)
                    {
                        chatRequest.Messages.Add(new ChatMessage
                        {
                            Role = ChatMessageRole.User,
                            Content = "请根据以上内容总结一句简短的标题"
                        });
                        var summary = await api.Chat.CreateChatCompletionAsync(chatRequest);
                        chatRequest.Messages.Remove(chatRequest.Messages.Last());
                        foreach (var item in historyChatRequestsList)
                        {
                            if (item.TimeId == oldChatTimeId)
                            {
                                item.History = new ChatRequest(chatRequest);
                                item.HistoryChatModel = new List<ChatModel>(ChatList.ToList());
                                item.HistoryTitle = summary.ToString();
                            }
                        }
                    }
                }
                string json = JsonConvert.SerializeObject(historyChatRequestsList);
                File.WriteAllText(path: savePath, json);
                //保存后清空界面和内存,重置唯一索引时间id
                oldChatTimeId = string.Empty;
                ChatList.Clear();
                LoadChatApiPara();
                WeakReferenceMessenger.Default.Send("", "ReloadHistoryList");
            }
        }
        [RelayCommand]
        public void OpenMask(ContentPage o)
        {
            MaskPopup maskPopup = new MaskPopup();
            o.ShowPopupAsync(maskPopup);
        }
        #endregion

        #region 属性
        /// <summary>
        /// AI和用户的所有对话框信息集合
        /// </summary>
        private ObservableCollection<ChatModel> _chatList;
        /// <summary>
        /// AI和用户的所有对话框信息集合
        /// </summary>
        public ObservableCollection<ChatModel> ChatList
        {
            get { return _chatList; }
            set { SetProperty(ref _chatList, value); }
        }

        /// <summary>
        /// 用户输入的文本
        /// </summary>
        private string _userText;
        /// <summary>
        /// 用户输入的文本
        /// </summary>
        public string UserText
        {
            get { return _userText; }
            set { SetProperty(ref _userText, value); }
        }
        #endregion
    }
}
