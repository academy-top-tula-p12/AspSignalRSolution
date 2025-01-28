namespace MauiSignalRApp
{
    public partial class MainPage : ContentPage
    {
        ChatModel chatModel;
        public MainPage()
        {
            InitializeComponent();
            chatModel = new ChatModel();

            BindingContext = chatModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await chatModel.Connect();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await chatModel.Disconnect();
        }


    }

}
