using CommunityToolkit.Maui.Views;
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

    private void OneMaskPopup_Closed(object sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
    {
        var maskJson = JsonConvert.SerializeObject(_viewmodel.MaskModelList.ToList());
        File.WriteAllText(_viewmodel.maskPath, maskJson);
    }
}