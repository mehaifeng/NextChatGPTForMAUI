using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views;

public partial class ChatPage : ContentPage
{
    ChatPageViewModel _viewModel;
    public ChatPage()
	{
		InitializeComponent();
        _viewModel = new ChatPageViewModel();
        BindingContext = _viewModel;
    }
}