using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WFTDC.Windows.Models
{
    public class Config  : INotifyPropertyChanged
    {

        private bool _isSaving;
        private bool _isSaveComplete;
        private string _accountNameInfo = "Select a grab mode and click the button above.";
        private string _cookie;
        private string _username;
        private bool _loggedIn;
        private bool _recieveMessages;
        private bool _setStatus;

        public string Cookie
        {
            get { return _cookie; }
            set { _cookie = value; OnPropertyChanged(); }
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set { _isSaving = value; OnPropertyChanged(); }
        }

        public bool IsSaveComplete
        {
            get { return _isSaveComplete; }
            set { _isSaveComplete = value; OnPropertyChanged(); }
        }

        public string AccountNameInfo
        {

            get { return _accountNameInfo; }
            set { _accountNameInfo = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public bool LoggedIn
        {
            get { return _loggedIn; }
            set { _loggedIn = value; OnPropertyChanged(); }
        }

        public bool SetStatus
        {
            get { return _setStatus; }
            set { _setStatus = value; OnPropertyChanged(); }
        }

        public bool RecieveMessages
        {
            get { return _recieveMessages; }
            set { _recieveMessages = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
