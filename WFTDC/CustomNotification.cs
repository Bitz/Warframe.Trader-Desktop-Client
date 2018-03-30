using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace WFTDC
{
    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        private CustomDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart =>
            _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public CustomNotification(string message)
        {
            Message = message;
        }



        public CustomNotification(string message, string image)
        {
            Message = message;
            Image = image;
        }

        private string _image;
        private string _message;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}