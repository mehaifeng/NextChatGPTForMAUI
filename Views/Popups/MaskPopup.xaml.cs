using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
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
        WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default, "ClearAllPreset");
        var maskJson = JsonConvert.SerializeObject(_viewmodel.MaskModelList.ToList());
        await File.WriteAllTextAsync(_viewmodel.maskPath, maskJson);
        if(!_viewmodel.isRemoved && !_viewmodel.isAdded)
        {
            return;
        }
        WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default, "ClearAllPreset");
        WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default, "LoadMaskModels");
    }
}