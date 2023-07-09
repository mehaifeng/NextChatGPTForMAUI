using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views;

public partial class ChatPage : ContentPage
{
    ChatPageViewModel _viewModel;
    public ChatPage(HistoryChatViewModel _historyChatViewModel)
	{
		InitializeComponent();
        _viewModel = new ChatPageViewModel();
        BindingContext = _viewModel;
    }
}