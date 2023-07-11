using CommunityToolkit.Maui.Views;
using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views.Popups;

public partial class MaskPopup : Popup
{
	public MaskPopup()
	{
		InitializeComponent();
		this.BindingContext = new MaskPopupViewModel();
	}
}