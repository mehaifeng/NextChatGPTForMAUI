using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using NextChatGPTForMAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class HistoryChatViewModel:ObservableObject
    {
        #region 本地私有属性
        private readonly string savePath = $"{FileSystem.Current.AppDataDirectory}/saveFile.json";
        private List<HistoryChatRequest> historyChatRequestsList = new();
        #endregion

        #region 构造函数
        public HistoryChatViewModel()
        {
            //当聊天界面保存了一个对话后，历史对话列表将会重载
            WeakReferenceMessenger.Default.Register<string,string>(this,"ReloadHistoryList", (r, m) =>
            {
                LoadHistoryChat();
                return;
            });
            LoadHistoryChat();
        }
        #endregion

        #region 函数
        /// <summary>
        /// 加载历史对话列表
        /// </summary>
        void LoadHistoryChat()
        {
            HistoryTitleList = new();
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                historyChatRequestsList = JsonConvert.DeserializeObject<List<HistoryChatRequest>>(json);
                foreach (var item in historyChatRequestsList)
                {
                    HistoryTitleList.Add(item.HistoryTitle);
                }
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 选择一个对话
        /// </summary>
        /// <param name="title"></param>
        [RelayCommand]
        public async Task HistoryChatSelect(string title)
        {
            var historyChat = historyChatRequestsList.Where(x => x.HistoryTitle == title).FirstOrDefault();
            if (historyChat != null)
            {
                WeakReferenceMessenger.Default.Send(historyChat,"ReloadHistoryChat");
                await Shell.Current.GoToAsync("//ChatPage");
            }
        }

        /// <summary>
        /// 删除一个对话
        /// </summary>
        /// <param name="title"></param>
        [RelayCommand]
        public void DeleteOneHistory(string title)
        {
            HistoryTitleList.Remove(title);
            historyChatRequestsList.Remove(historyChatRequestsList.Where(x => x.HistoryTitle == title).FirstOrDefault());
            string json = JsonConvert.SerializeObject(historyChatRequestsList);
            File.WriteAllTextAsync(savePath, json);
            WeakReferenceMessenger.Default.Send(historyChatRequestsList, "RefreshHistoryChatList");
        }
        #endregion


        #region 属性
        /// <summary>
        /// 历史对话列表
        /// </summary>
        private ObservableCollection<string> _historyTitleList;
        /// <summary>
        /// 历史对话列表
        /// </summary>
        public ObservableCollection<string> HistoryTitleList
        {
            get { return _historyTitleList; }
            set { SetProperty(ref  _historyTitleList, value); }
        }
        /// <summary>
        /// 历史标题
        /// </summary>
        private string _historyTitle;
        /// <summary>
        /// 历史标题
        /// </summary>
        public string HistoryTitle
        {
            get { return _historyTitle; }
            set { SetProperty(ref _historyTitle, value); }
        }
        #endregion
    }
}
