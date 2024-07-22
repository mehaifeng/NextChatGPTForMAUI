using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextChatGPTForMAUI.Viewmodels;
using NextChatGPTForMAUI.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NextChatGPTForMAUI.Models
{
    public partial class ChatModel:ObservableObject
    {
        [ObservableProperty]
        private string tag;
        [ObservableProperty]
        private string text;
        [ObservableProperty]
        private bool isUser;
        [ObservableProperty]
        private string aiFace;
        [ObservableProperty]
        private string userFace;
        [ObservableProperty]
        private bool isReadOnly;
        [ObservableProperty]
        private bool messageMenuState;

        /// <summary>
        /// 显示菜单
        /// </summary>
        [RelayCommand]
        public void ShowChatMenu(HorizontalStackLayout o)
        {
            MessageMenuState = true;
            foreach (var item in ChatPageViewModel.GetInstance().ChatList)
            {
                if(item != this)
                {
                    item.MessageMenuState = false;
                }
            }
        }
        /// <summary>
        /// 关闭显示菜单
        /// </summary>
        [RelayCommand]
        public void CloseMessageMenu()
        {
            MessageMenuState = false;
        }
        /// <summary>
        /// 复制消息
        /// </summary>
        [RelayCommand]
        public void CopyMessage()
        {
            Clipboard.Default.SetTextAsync(Text);
            MessageMenuState = false;
        }
        /// <summary>
        /// 编辑消息
        /// </summary>
        [RelayCommand]
        public void EditMessage(Editor o)
        {
            IsReadOnly = false;
            o.ShowKeyboardAsync();
            MessageMenuState = false;
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        [RelayCommand]
        public void DeleteMessage()
        {
            var deleteItem = ChatPageViewModel.GetInstance().ChatList.First(t => t == this);
            ChatPageViewModel.GetInstance().ChatList.Remove(deleteItem);
        }
    }
}
