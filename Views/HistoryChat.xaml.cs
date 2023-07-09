using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views;

public partial class HistoryChat : ContentPage
{
    public HistoryChat()
	{
        InitializeComponent();
        BindingContext = new HistoryChatViewModel(); 
    }
}