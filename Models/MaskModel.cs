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
        [ObservableProperty]
        private int selectIndex;
        [ObservableProperty]
        private string text;
    }
}
