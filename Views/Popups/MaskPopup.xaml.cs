using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using NextChatGPTForMAUI.Models;
using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views.Popups;

public partial class MaskPopup : Popup
{
    readonly MaskPopupViewModel _viewmodel;

    public MaskPopup()
	{
		InitializeComponent();
        _viewmodel = new MaskPopupViewModel();
        BindingContext = _viewmodel;
    }

    private async void OneMaskPopup_Closed(object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
    {
        MaskType masks = _viewmodel.SelectedMask;
        List<MaskType> maskTypeList = _viewmodel.MaskTypeList.ToList();
        var maskJson = JsonConvert.SerializeObject(maskTypeList);
        await File.WriteAllTextAsync(_viewmodel.maskPath, maskJson);
        WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default, "ClearAllPreset");
        WeakReferenceMessenger.Default.Send(masks, "LoadMaskModels");
    }
}