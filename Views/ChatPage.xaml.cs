using NextChatGPTForMAUI.Viewmodels;
using NextChatGPTForMAUI.Views.Templates;

namespace NextChatGPTForMAUI.Views;

public partial class ChatPage : ContentPage
{
    ChatPageViewModel _viewModel;
    public ChatPage()
	{
		InitializeComponent();
        _viewModel = ChatPageViewModel.GetInstance();
        BindingContext = _viewModel;
        SetupChatTemplates();
    }
    private void SetupChatTemplates()
    {
        //设置资源目录
        var resources = new ResourceDictionary();
        resources.Add("UserChatTemplate", new DataTemplate(() => new UserChatTemplate()));
        resources.Add("AiChatTemplate", new DataTemplate(() => new AiChatTemplate(_viewModel)));
        //设置资源选择器
        var ChatTemplateSelector = new ChatTemplateSelector
        {
            UserChatTemplate = (DataTemplate)resources["UserChatTemplate"],
            AiChatTemplate = (DataTemplate)resources["AiChatTemplate"]
        };
        ChatCollectionView.ItemTemplate = ChatTemplateSelector;
        ThisChat.Resources.MergedDictionaries.Add(resources);
    }
}