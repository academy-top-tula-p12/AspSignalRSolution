using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace MauiSignalRApp
{
    public class ChatModel : INotifyPropertyChanged
    {
        public ChatModel()
        {
            hub = new HubConnectionBuilder().WithUrl("https://localhost:7244/chat")
                                            .Build();

            IsBusy = false;
            IsConnected = false;

            hub.On<string, string>("Reseive", AddMessage);

            hub.Closed += async (error) =>
            {
                AddMessage("", "Disconnect");
                IsConnected = false;
                await Task.Delay(1000);
                await Connect();
            };

            SendMessageCommand = new Command(
                                            async () => await SendMessage(),
                                            () => IsConnected);

        }


        HubConnection hub;
        public string? UserName { get; set; }
        public string? Message { get; set; }

        public ObservableCollection<MessageData> Messages { get; set; } = new();

        bool isBusy;
        public bool IsBusy
        {
            get => isBusy; 
            set
            {
                if(isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        public Command SendMessageCommand { get; }

        public async Task SendMessage()
        {
            try
            {
                IsBusy = true;
                await hub.InvokeAsync("SendMessage", UserName, Message);
            }
            catch (Exception ex)
            {
                AddMessage("", ex.Message);
            }

            IsBusy = false;
        }

        public async Task Connect()
        {
            if (IsConnected)
                return;

            try
            {
                await hub.StartAsync();
                AddMessage("", "You logget to chat");

                IsConnected = true;
            }
            catch (Exception ex)
            {
                AddMessage("", ex.Message);
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected) return;

            await hub.StopAsync();
            IsConnected = false;
            AddMessage("", "You leave chat");
        }

        void AddMessage(string username, string message)
        {
            Messages.Insert(0, new() { Name = username, Message = message });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MessageData
    {
        public string? Name { get; set; }
        public string? Message { get; set; }
    }
}
