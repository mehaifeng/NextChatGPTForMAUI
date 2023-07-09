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
        [RelayCommand]
        public async void HistoryChatSelect(string title)
        {
            var historyChat = historyChatRequestsList.Where(x => x.HistoryTitle == title).FirstOrDefault();
            if (historyChat != null)
            {
                WeakReferenceMessenger.Default.Send(historyChat,"ReloadHistoryChat");
                await Shell.Current.GoToAsync("//ChatPage");
            }
        }
        #endregion


        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> historyTitleList;

        [ObservableProperty]
        private string historyTitle;
        #endregion
    }
}
