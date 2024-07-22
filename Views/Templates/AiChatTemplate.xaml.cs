using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.Messaging;
using NextChatGPTForMAUI.Models;
using NextChatGPTForMAUI.Viewmodels;

namespace NextChatGPTForMAUI.Views.Templates;

public partial class AiChatTemplate : Grid
{
    public AiChatTemplate(ChatPageViewModel chatPageViewModel)
	{
		InitializeComponent();
        BindingContext = chatPageViewModel;
    }

    double movement { get; set; }
    private void AiBox_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        SwipeView swipeView = sender as SwipeView;
        if (Math.Abs(movement) > 0.5 * swipeView.Threshold)
        {
            ChatModel item = BindingContext as ChatModel;
            if(item.Text != "Thinking...")
            {
                WeakReferenceMessenger.Default.Send(item, "RemoveSingleChat");
            }
        }
    }

    private void Editor_Unfocused(object sender, FocusEventArgs e)
    {
        ChatModel item = BindingContext as ChatModel;
        item.IsReadOnly = true;
        AiEditor.HideKeyboardAsync();
        if (item.Text != "Thinking...")
        {
            WeakReferenceMessenger.Default.Send(item, "UpdateChatText");
        }
    }
}