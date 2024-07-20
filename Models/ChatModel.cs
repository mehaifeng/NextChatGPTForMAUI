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
        private bool isReadOnly = true;
        [ObservableProperty]
        private bool messageMenuState = false;

        /// <summary>
        /// 显示菜单
        /// </summary>
        [RelayCommand]
        public void ShowChatMenu(HorizontalStackLayout o)
        {
            o.Focus();
            MessageMenuState = true;
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
        }
        /// <summary>
        /// 编辑消息
        /// </summary>
        [RelayCommand]
        public void EditMessage(Editor o)
        {
            IsReadOnly = false;
            o.Focus();
            o.ShowKeyboardAsync();
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
