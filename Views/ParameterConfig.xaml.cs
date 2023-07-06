using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views;

public partial class ParameterConfig : ContentPage
{
	public ParameterConfig()
	{
		InitializeComponent();
		this.BindingContext = new ParameterConfigViewModel();
    }
}