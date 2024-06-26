using CommunityToolkit.Mvvm.Messaging;
using NextChatGPTForMAUI.Models;

namespace NextChatGPTForMAUI.Views.Templates;

public partial class UserChatTemplate : Grid
{
	public UserChatTemplate()
	{
		InitializeComponent();
	}
    double movement { get; set; }
    private void UserBox_SwipeChanging(object sender, SwipeChangingEventArgs e)
    {
        SwipeView swipeView = sender as SwipeView;
        movement = e.Offset;
        if (Math.Abs(movement) > 0.5 * swipeView.Threshold)
        {
            MessageBox.Opacity = 0.5;
        }
        else
        {
            MessageBox.Opacity = 1;
        }
    }
    private void UserBox_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        SwipeView swipeView = sender as SwipeView;
        if (Math.Abs(movement) > 0.5 * swipeView.Threshold)
        {
            ChatModel item = BindingContext as ChatModel;
            WeakReferenceMessenger.Default.Send(item, "RemoveSingleChat");
        }
    }

    private void Editor_Unfocused(object sender, FocusEventArgs e)
    {
        ChatModel item = BindingContext as ChatModel;
        WeakReferenceMessenger.Default.Send(item, "UpdateChatText");
    }
}