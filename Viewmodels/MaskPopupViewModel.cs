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
    public partial class MaskPopupViewModel:MaskType
    {
        public readonly string maskPath = $"{FileSystem.Current.AppDataDirectory}/maskfile.json";
        public bool isAdded;
        public bool isRemoved;
        public MaskPopupViewModel()
        {
            MaskModels = new ObservableCollection<MaskModel>();
            MaskTypeList = new ObservableCollection<MaskType>();
            //如果有预设文件则读取预设
            if (File.Exists(maskPath))
            {
                string maskJson = File.ReadAllText(maskPath);
                List<MaskType> masks = new(JsonConvert.DeserializeObject<List<MaskType>>(maskJson));
                MaskTypeList = new ObservableCollection<MaskType>(masks);
                SelectedMask = MaskTypeList.First(t=>t.IsLastUsed);
            }
            //否则增加一条空预设
            else
            {
                MaskTypeList.Add(new MaskType
                {
                     MaskModels = new ObservableCollection<MaskModel>(),
                     IsLastUsed = true,
                     MaskFace = "😊",
                     MaskName = "新预设"
                });;
                SelectedMask = MaskTypeList.First();
                AddMaskModel();
            }
            isAdded = false;
            isRemoved = false;
        }
        #region 可绑定属性
        /// <summary>
        /// 面具预设合集
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MaskType> maskTypeList;
        [ObservableProperty]
        private MaskType selectedMask;
        #endregion

        #region 命令
        /// <summary>
        /// 新增预设面具
        /// </summary>
        [RelayCommand]
        public void AddMaskType()
        {
            MaskTypeList.Add(new MaskType
            {
                MaskFace = "😊",
                MaskModels = new ObservableCollection<MaskModel>(),
                MaskName = "新预设"
            });
            SelectedMask = MaskTypeList.Last();
        }
        [RelayCommand]
        public void RemoveMaskType()
        {
            MaskTypeList.Remove(SelectedMask);
            SelectedMask = MaskTypeList.Last();
        }
        /// <summary>
        /// 选择面具预设
        /// </summary>
        [RelayCommand]
        public void SelectMask()
        {
            foreach(var item in MaskTypeList)
            {
                if(item == SelectedMask)
                {
                    item.IsLastUsed = true;
                }
                else
                {
                    item.IsLastUsed = false;
                }
            }
        }

        /// <summary>
        /// 新增一条预设对话MaskModel
        /// </summary>
        [RelayCommand]
        public void AddMaskModel()
        {
            MaskModel newMaskModel = new() { SelectIndex = 0, Text = string.Empty };
            MaskModels.Add(newMaskModel);
            SelectedMask.MaskModels.Add(newMaskModel);
            isAdded = true;
        }
        /// <summary>
        /// 移除MaskModel
        /// </summary>
        /// <param name="o"></param>
        [RelayCommand]
        public void RemoveMaskModel(MaskModel o)
        {
            SelectedMask.MaskModels.Remove(o);
            isRemoved = true;
        }
        #endregion
    }
}
