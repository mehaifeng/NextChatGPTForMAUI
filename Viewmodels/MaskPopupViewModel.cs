using CommunityToolkit.Maui.Views;
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
    public partial class MaskPopupViewModel:ObservableObject
    {
        public readonly string maskPath = $"{FileSystem.Current.AppDataDirectory}/maskfile.json";
        public MaskPopupViewModel()
        {
            MaskModelList = new ObservableCollection<MaskModel>();
            if (File.Exists(maskPath))
            {
                string maskJson = File.ReadAllText(maskPath);
                List<MaskModel> masks = new(JsonConvert.DeserializeObject<List<MaskModel>>(maskJson));
                MaskModelList = new ObservableCollection<MaskModel>(masks);
            }
            else
            {
                MaskModelList = new ObservableCollection<MaskModel> { new MaskModel { SelectIndex = 0 , Text = string.Empty} };
            }
        }
        #region 可绑定属性
        [ObservableProperty]
        private ObservableCollection<MaskModel> maskModelList;
        #endregion

        #region 命令
        /// <summary>
        /// 新增MaskModel
        /// </summary>
        [RelayCommand]
        public void AddMaskModel()
        {
            MaskModel newMaskModel = new() { SelectIndex = 0, Text = string.Empty };
            MaskModelList.Add(newMaskModel);
            WeakReferenceMessenger.Default.Send(MaskModelList.ToList(), "AddThisPreset");
        }
        /// <summary>
        /// 移除MaskModel
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void RemoveMaskModel(MaskModel o)
        {
            MaskModelList.Remove(o);
            WeakReferenceMessenger.Default.Send(o, "RemoveThisPreset");
        }
        #endregion
    }
}
