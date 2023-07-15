using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public partial class MaskModel:ObservableObject
    {
        /// <summary>
        /// 选择角色索引，0是System、1是User、2是Assistant
        /// </summary>
        [ObservableProperty]
        private int selectIndex;
        /// <summary>
        /// 对话预设
        /// </summary>
        [ObservableProperty]
        private string text;
    }
}
