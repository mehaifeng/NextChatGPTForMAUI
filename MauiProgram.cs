using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Autofac;
using NextChatGPTForMAUI.Viewmodels;
using NextChatGPTForMAUI.Views;

namespace NextChatGPTForMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
			.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<ChatPage>();
		builder.Services.AddSingleton<HistoryChatViewModel>();
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
