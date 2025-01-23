using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace WpfSignalRClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hub;
        public MainWindow()
        {
            InitializeComponent();

            hub = new HubConnectionBuilder().WithUrl("https://localhost:7244/chat").Build();

            hub.On<string, string, string>("Reseive", (user, message, connectionId) =>
            {
                Dispatcher.Invoke(() =>
                {
                    message = $"{user} {connectionId}: {message}";
                    chatRoom.Items.Insert(0, message);
                    chatRoom.Items.Refresh();
                });
            });

            hub.On<string>("Notify", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    chatRoom.Items.Insert(0, message);
                    chatRoom.Items.Refresh();
                });
            });
        }

        

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await hub.InvokeAsync("SendMessage", user.Text, message.Text);
                message.Text = "";
            }
            catch(Exception ex)
            {
                chatRoom.Items.Add(ex.Message);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await hub.StartAsync();
                //chatRoom.Items.Add("In chat");
                btnSend.IsEnabled = true;
            }
            catch(Exception ex)
            {
                chatRoom.Items.Add(ex.Message);
            }
        }
    }
}