using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using NextChatGPTForMAUI.Models;

namespace NextChatGPTForMAUI.Viewmodels
{
    public partial class ParameterConfigViewModel : ObservableObject
    {
        private readonly string path = FileSystem.Current.AppDataDirectory + "/parameter.json";
        #region 构造函数
        public ParameterConfigViewModel()
        {
            ParameterModel = new ParameterModel();
            Initial();
        }
        #endregion

        #region 属性
        [ObservableProperty]
        ParameterModel parameterModel;
        #endregion

        #region 函数
        private void Initial()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ParameterModel = JsonConvert.DeserializeObject<ParameterModel>(json);
            }
        }
        #endregion

        #region 命令
        [RelayCommand]
        private void Save()
        {
            //保存到本地
            string str = JsonConvert.SerializeObject(ParameterModel);
            File.WriteAllText(path, str);
            //保存完成后显示toast提示
            var toast = Toast.Make("保存成功");
            toast.Show();
            WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default, "ParameterConfigSetup");
        }
        #endregion
    }
}
