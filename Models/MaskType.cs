using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Models
{
    public partial class MaskType : ObservableObject
    {
        /// <summary>
        /// 面具预设信息
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MaskModel> maskModels;

        /// <summary>
        /// AI面具脸谱
        /// </summary>
        [ObservableProperty]
        private string maskFace;

        /// <summary>
        /// 面具名称
        /// </summary>
        [ObservableProperty]
        private string maskName;

        /// <summary>
        /// 用户面具脸谱
        /// </summary>
        [ObservableProperty]
        private string userFace;

        /// <summary>
        /// 上一次是否使用过
        /// </summary>
        [ObservableProperty]
        private bool isLastUsed = false;
    }
}
