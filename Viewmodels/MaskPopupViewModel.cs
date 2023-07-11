using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NextChatGPTForMAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class MaskPopupViewModel:ObservableObject
    {
        public MaskPopupViewModel()
        {
            maskModelList = new ObservableCollection<MaskModel>
            {
                new MaskModel() { Text = "abcdefghqwdw", SelectIndex = 0 },
                new MaskModel() { Text = "adeqdwqedqed", SelectIndex = 1 },
                new MaskModel() { Text = "qwdqefergerg", SelectIndex = 2 }
            };
        }
        #region 可绑定属性
        [ObservableProperty]
        private ObservableCollection<MaskModel> maskModelList;
        #endregion

        #region 命令
        [RelayCommand]
        public void AddMaskModel()
        {
            MaskModelList.Add(new MaskModel() { Text = "qwdqefergerg", SelectIndex = 2 });
        }
        #endregion
    }
}
