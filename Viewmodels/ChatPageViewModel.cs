using NextChatGPTForMAUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using System.Security.Authentication;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using NextChatGPTForMAUI.Views;
using NextChatGPTForMAUI.Views.Popups;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using NextChatGPTForMAUI.Tools;
using NextChatGPTForMAUI.Views.Templates;
using Microsoft.Maui.Controls.Platform;
using System.Diagnostics;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class ChatPageViewModel:ChatRequestHttp
    {
        private static ChatPageViewModel Instance;
        public static ChatPageViewModel GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ChatPageViewModel();
            }
            return Instance;
        }

        #region 本地私有属性
        private readonly string path = $"{FileSystem.Current.AppDataDirectory}/parameter.json";
        private readonly string savePath = $"{FileSystem.Current.AppDataDirectory}/saveFile.json";
        private readonly string maskPath = $"{FileSystem.Current.AppDataDirectory}/maskfile.json";
        private ChatRequest chatRequest;
        private List<HistoryChatRequest> historyChatRequestsList;
        private ParameterModel paraConfig;
        private TaskCompletionSource taskCompletionSource;
        private string oldChatTimeId;
        #endregion

        #region 构造函数
        public ChatPageViewModel()
        {
            #region 各种初始化
            ChatList = new ObservableCollection<ChatModel>();
            historyChatRequestsList = new List<HistoryChatRequest>();
            ChatPageInitial();
            #endregion

            #region 注册消息
            //载入聊天配置
            WeakReferenceMessenger.Default.Register<WeakReferenceMessenger, string>(this, "ParameterConfigSetup", (r, m) =>
            {
                ChatPageInitial();
            });
            //载入历史对话
            WeakReferenceMessenger.Default.Register<HistoryChatRequest, string>(this, "ReloadHistoryChat", (r, m) =>
            {
                oldChatTimeId = m.TimeId;
                //加载对话带视图界面
                ChatList = new ObservableCollection<ChatModel>(m.HistoryChatModel);
                //加载对话到消息队列
                foreach (ChatModel chatModel in ChatList)
                {
                    chatRequest.messages.Add(new ChatMessage
                    {
                        content = chatModel.Text,
                        role = chatModel.IsUser ? "user" : "assistant",
                        IsMaskType = false
                    });
                }
            });
            //刷新历史对话列表
            WeakReferenceMessenger.Default.Register<List<HistoryChatRequest>,string>(this,"RefreshHistoryChatList",(r,m)=>
            {
                historyChatRequestsList = m;
            });
            //移除一条消息
            WeakReferenceMessenger.Default.Register<ChatModel, string>(this, "RemoveSingleChat", (r, m) =>
            {
                chatRequest.messages.Remove(chatRequest.messages.FirstOrDefault(x => x.content == m.Text && x.role == (m.IsUser == true ? "user" : "assistant")));
                ChatList.Remove(m);
            });
            //清空预设
            WeakReferenceMessenger.Default.Register<WeakReferenceMessenger, string>(this, "ClearAllPreset", (r, m) =>
            {
                if (chatRequest.messages.Count > 0)
                {
                    List<ChatMessage> temps = new(chatRequest.messages);
                    foreach (var item in temps)
                    {
                        if (item.IsMaskType)
                        {
                            chatRequest.messages.Remove(item);
                        }
                        else if (!item.IsMaskType)
                        {
                            break;
                        }
                    }
                }
            });
            //重载预设
            WeakReferenceMessenger.Default.Register<MaskType, string>(this, "LoadMaskModels", (r, m) =>
            {
                LoadMaskModelInfos(m);
            });
            //更新对话
            WeakReferenceMessenger.Default.Register<ChatModel, string>(this, "UpdateChatText", (r, m) =>
            {
                int index = ChatList.IndexOf(m);
                int systemSum = chatRequest.messages.Where(t => t.role=="system").Count();
                chatRequest.messages[index + systemSum].content = m.Text;
            });
            #endregion
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
            }
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                historyChatRequestsList = JsonConvert.DeserializeObject<List<HistoryChatRequest>>(json);
            }
            //初始化ChatAPI参数/重新读取配置并加载到ChatAPI参数
            LoadChatApiPara();
            LoadMaskModelInfos(null);
        }
        /// <summary>
        /// 装载ChatApi参数
        /// </summary>
        private void LoadChatApiPara()
        {
            chatRequest = new ChatRequest()
            {
                model = string.IsNullOrEmpty(paraConfig?.Model) ?
                    "gpt-3.5-turbo" : paraConfig?.Model,

                temperature = string.IsNullOrEmpty(paraConfig?.Temperature) ?
                    1 : Convert.ToDouble(paraConfig?.Temperature),

                top_p = string.IsNullOrEmpty(paraConfig?.Top_p) ?
                    1 : Convert.ToDouble(paraConfig?.Top_p),

                frequency_penalty = string.IsNullOrEmpty(paraConfig?.Frequency_penalty) ?
                    0 : Convert.ToDouble(paraConfig?.Frequency_penalty),

                presence_penalty = string.IsNullOrEmpty(paraConfig?.Presence_penalty) ?
                    0 : Convert.ToDouble(paraConfig?.Presence_penalty),

                max_tokens = string.IsNullOrEmpty(paraConfig?.Max_tokens) ?
                    2000 : Convert.ToInt32(paraConfig?.Max_tokens),

                messages = new List<ChatMessage>(),

                stream = true
            };
        }
        /// <summary>
        /// 新加载面具预设信息
        /// </summary>
        private void LoadMaskModelInfos(MaskType masks)
        {
            int i = 0;
            if (masks==null && File.Exists(maskPath))
            {
                string maskJson = File.ReadAllText(maskPath);
                List<MaskType> maskTypes = JsonConvert.DeserializeObject<List<MaskType>>(maskJson);
                foreach(var item in maskTypes)
                {
                    if (item.IsLastUsed)
                    {
                        foreach(var maskModel in item.MaskModels)
                        {
                            chatRequest.messages.Insert(i, new ChatMessage()
                            {
                                content = maskModel.Text,
                                role = maskModel.SelectIndex == 0 ? "system" : (maskModel.SelectIndex == 1 ? "user" : "assistant"),
                                IsMaskType = true
                            });
                            i++;
                        }
                        break;
                    }
                }
            }
            if (masks != null)
            {
                foreach(var item in ChatList)
                {
                    if (item.IsUser)
                    {
                        item.UserFace = masks.UserFace;
                    }
                    else
                    {
                        item.AiFace = masks.MaskFace;
                    }
                }
                foreach (var item in masks.MaskModels)
                {
                    chatRequest.messages.Insert(i, new ChatMessage()
                    {
                        content = item.Text,
                        role = item.SelectIndex == 0 ? "system" : (item.SelectIndex == 1 ? "user" : "assistant"),
                        IsMaskType = true
                    });
                    i++;
                }
            }
        }
        /// <summary>
        /// 显示回复
        /// </summary>
        private async void WillShowResultFromAPI(ContentPage page, Border o, CollectionView chatView)
        {
            bool isCorrect = true;
            bool isDeleteThinking = false;
            taskCompletionSource = new TaskCompletionSource();
            ChatModel AiRespondModel = new()
            {
                Text = "Thinking...",
                IsUser = false,
                MessageMenuState = false,
                IsReadOnly = true
            };
            ChatList.Add(AiRespondModel);
            Thread thread = new(async () =>
            {
                try
                {
                    await foreach(string text in ReceiveStreamResponseFromOpenAI(chatRequest,paraConfig.Api_address, paraConfig.Apikey))
                    {
                        if (!isDeleteThinking)
                        {
                            AiRespondModel.Text = string.Empty;
                            isDeleteThinking = true;
                        }
                        System.Threading.Thread.Sleep(20);
                        AiRespondModel.Text += text;
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
                chatRequest.messages.Add(new ChatMessage
                {
                    role = "assistant",
                    content = AiRespondModel.Text
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
        public async Task Send(object[] controls)
        {
            ContentPage page = controls[0] as ContentPage;
            Border o = controls[1] as Border;
            CollectionView chatView = controls[2] as CollectionView;
            //如果用户没有输入文本，则不发送
            if(string.IsNullOrEmpty(UserText))
            {
                return;
            }
            //如果用户没有设置ApiKey参数，则不发送
            if(paraConfig==null||string.IsNullOrEmpty(paraConfig?.Apikey))
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
            //正式发送前，检查请求队列有无空消息，有则移除
            List<ChatMessage> temps = new(chatRequest.messages);
            foreach(var item in temps)
            {
                if (string.IsNullOrEmpty(item.content))
                {
                    chatRequest.messages.Remove(item);
                }
            }
            temps.Clear();
            //将发送的内容添加到视图界面
            ChatList.Add(new ChatModel
            {
                IsUser = true,
                Text = UserText,
                MessageMenuState = false,
                IsReadOnly = true
            }) ;
            //将发送的内容添加到请求队列
            chatRequest.messages.Add(new ChatMessage
            {
                role = "user",
                content = UserText
            });
            //用户发送后，清空输入框
            UserText = string.Empty;
            WillShowResultFromAPI(page, o, chatView);
        }
        /// <summary>
        /// 清空/保存
        /// </summary>
        [RelayCommand]
        public async Task ClearAndSave(ContentPage o)
        {
            string action = await o.DisplayActionSheet("清除消息", "取消", null, "保存并清除", "只清除");
            if (action == "取消") { return; }
            else if (action == "只清除" || taskCompletionSource == null)
            {
                oldChatTimeId = string.Empty;
                if (ChatList.Count > 0)
                {
                    foreach (var chat in ChatList)
                    {
                        chat.MessageMenuState = false;
                        chat.IsReadOnly = true;
                    }
                    ChatList.Clear();
                    ChatList = new ObservableCollection<ChatModel>([]);
                    chatRequest.messages.Clear();
                }
            }
            else if (action == "保存并清除")
            {
                if (ChatList.Count > 0 && taskCompletionSource.Task.IsCompleted)
                {
                    if (historyChatRequestsList.Count == 0)
                    {
                        oldChatTimeId = string.Empty;
                    }
                    if (string.IsNullOrEmpty(oldChatTimeId))
                    {
                        chatRequest.messages.Add(new ChatMessage
                        {
                            role = "user",
                            content = "这是一条来自system的指令：请根据以上对话内容总结一句简短的标题，50字以内"
                        });
                        string summary = null;
                        await foreach (string text in ReceiveStreamResponseFromOpenAI(chatRequest, paraConfig.Api_address, paraConfig.Apikey))
                        {
                            summary += text;
                        }
                        chatRequest.messages.Remove(chatRequest.messages.Last());
                        foreach (var eachChat in ChatList)
                        {
                            eachChat.MessageMenuState = false;
                            eachChat.IsReadOnly = true;
                        }
                        historyChatRequestsList.Add(new HistoryChatRequest
                        {
                            TimeId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                            HistoryTitle = summary.ToString(),
                            History = chatRequest,
                            HistoryChatModel = ChatList.ToList()
                        });
                    }
                    else if (historyChatRequestsList.Any(t => t.TimeId == oldChatTimeId))
                    {
                        if (historyChatRequestsList.First(t => t.TimeId == oldChatTimeId).HistoryChatModel.Count != ChatList.Count)
                        {
                            chatRequest.messages.Add(new ChatMessage
                            {
                                role = "user",
                                content = "这是一条作为system的指令：请根据以上内容总结一句简短的标题，50字以内"
                            });
                            string summary = null;
                            await foreach (string text in ReceiveStreamResponseFromOpenAI(chatRequest, paraConfig.Api_address, paraConfig.Apikey))
                            {
                                summary += text;
                            }
                            chatRequest.messages.Remove(chatRequest.messages.Last());
                            foreach (var item in historyChatRequestsList)
                            {
                                if (item.TimeId == oldChatTimeId)
                                {
                                    item.History = chatRequest;
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
                    ChatList = new ObservableCollection<ChatModel>([]);
                    //ChatList.Clear();
                    LoadChatApiPara();
                    LoadMaskModelInfos(null);
                    WeakReferenceMessenger.Default.Send("", "ReloadHistoryList");
                }
            }
        }
        /// <summary>
        /// 打开面具预设Popup
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task OpenMask(ContentPage o)
        {
            MaskPopup maskPopup = new();
            await o.ShowPopupAsync(maskPopup);
        }
        /// <summary>
        /// 显示消息菜单
        /// </summary>
        [RelayCommand]
        public void ShowMessageMenu(Grid o)
        {
            ChatModel temp = o.BindingContext as ChatModel;
            temp.MessageMenuState = true;
        }
        /// <summary>
        /// 关闭消息菜单
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void CloseMessageMenu(Grid o)
        {
            ChatModel temp = o.BindingContext as ChatModel;
            temp.MessageMenuState = false;
        }
        /// <summary>
        /// 编辑消息
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void EditorMessage(Grid o)
        {
            ChatModel temp = o.BindingContext as ChatModel;
            ChatList.First(t => t == temp).IsReadOnly = true;
            ChatList.First(t => t == temp).MessageMenuState = false;
        }
        /// <summary>
        /// 复制消息到剪切板
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void CopyMessage(Grid o)
        {
            ChatModel temp = o.BindingContext as ChatModel;
            Clipboard.Default.SetTextAsync(temp.Text);
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void DeleteMessage(Grid o)
        {
            ChatModel temp = o.BindingContext as ChatModel;
            ChatList.Remove(temp);
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
