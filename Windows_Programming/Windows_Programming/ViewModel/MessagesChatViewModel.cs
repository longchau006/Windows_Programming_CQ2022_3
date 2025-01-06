using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class MessagesChatViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                    OnPropertyChanged(nameof(Messages));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void Init()
        {
            Messages.Clear();
            Messages.Add(new Message { Content = "Hello! How can I help you today?", IsAI = true, TimeMessage=DateTime.Now });
            //Messages.Add(new Message { Content = "I need help with booking a tour", IsAI = false });
            //new Message { Content = "I'd be happy to help you book a tour. What destination are you interested in?", IsAI = true },
            //new Message { Content = "I'm thinking about visiting Paris", IsAI = false },
            //new Message { Content = "Excellent choice! Paris is beautiful this time of year. Would you like to see our available Paris tour packages?", IsAI = true })
            OnPropertyChanged(nameof(Messages));
        }
        public void AddNewMessageInHome(Message newMessage)
        {

            Messages.Add(newMessage);
            OnPropertyChanged(nameof(Messages)); // Thông báo rằng PlansInHome đã thay đổi
        }
        public void DeleteAllHistoryChat()
        {

            Messages.Clear();
            OnPropertyChanged(nameof(Messages)); // Thông báo rằng PlansInHome đã thay đổi
        }
    }
}

