using CommunityToolkit.Mvvm.ComponentModel;

namespace NextChatGPTForMAUI.Models
{
    public partial class ParameterModel : ObservableObject
    {
        [ObservableProperty]
        private string model = "gpt-3.5-turbo-16k";
        [ObservableProperty]
        private string temperature;
        [ObservableProperty]
        private string top_p;
        [ObservableProperty]
        private string frequency_penalty;
        [ObservableProperty]
        private string presence_penalty;
        [ObservableProperty]
        private string max_tokens;
        [ObservableProperty]
        private string api_address;
        [ObservableProperty]
        private string apikey;
    }
}
