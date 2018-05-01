namespace WFTDC.Windows
{
    using System.ComponentModel;
    using System.Net;
    using System.Windows.Controls;
    using static Cookie;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config
    {
        readonly Models.Config _config = new Models.Config();

        public Config()
        {
            InitializeComponent();
            DataContext = _config;
            Check_startWithWindows.IsChecked = Utils.IsStartUpEnabled();
            Check_ToggleWatcher.IsChecked = Global.Configuration.Application.Watcher;
            LB_AccountMode.SelectedIndex = (int) Global.Configuration.User.Account.GetCookieFrom;
            _config.Cookie = Global.Configuration.User.Account.Cookie;
            _config.SetStatus = Global.Configuration.User.Account.SetStatus;
            _config.RecieveMessages = Global.Configuration.User.Account.GetMessages;
            if (!string.IsNullOrEmpty(Global.Configuration.User.Account.Cookie))
            {
                _config.LoggedIn = true;
                _config.IsSaveComplete = true;
                _config.AccountNameInfo = $"Signed in as {Global.Configuration.User.Account.Username}";
            }
            else
            {
                _config.IsSaveComplete = false;
            }
        }

        private void Check_startWithWindows_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox) sender;

            if (checkBox.IsChecked.Value)
            {
                Utils.StartWithWindows();
            }
            else
            {
                Utils.DoNotStartWithWindows();
            }
            if (Global.Configuration.Application == null)
            {
                Global.Configuration.Application = new C.Application();
            }
            Global.Configuration.Application.StartWithWindows = checkBox.IsChecked.Value;
            Functions.Config.Save();
        }

        private void Grab_Account_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = LB_AccountMode.SelectedIndex;
            _config.IsSaving = true;
            _config.IsSaveComplete = false;

            BackgroundWorker bw = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            bw.DoWork += delegate {
                GetAccount((C.Account.GetCookieFromEnum) selectedIndex);
            };
            bw.RunWorkerAsync();

            bw.RunWorkerCompleted += (o, args) =>
            {
                _config.IsSaving = false;
                if (!string.IsNullOrEmpty(_config.Username))
                {
                    _config.AccountNameInfo = $"Signed in as {_config.Username}";
                    _config.IsSaveComplete = true;
                }
                else
                {
                    _config.AccountNameInfo = string.Empty;
                }
            };
        }

        public bool GetAccount(C.Account.GetCookieFromEnum cookieMode)
        {
            bool result = false;
            string value = string.Empty;
            switch (cookieMode)
            {
                case C.Account.GetCookieFromEnum.Chrome:
                    GetCookieFromChrome(".warframe.market", "JWT", ref value);
                    break;
                case C.Account.GetCookieFromEnum.Firefox:
                    GetCookieFromFirefox("warframe.market", "JWT", ref value);
                    break;
                case C.Account.GetCookieFromEnum.InternetExplorer:
                    GetCookieFromInternetExplorer("warframe.market", "JWT", ref value);
                    break;
                case C.Account.GetCookieFromEnum.ManualEntry:
                    value = _config.Cookie;
                    break;
            }

            var userName = GetUsernameUsingCookie(value);

            if (!string.IsNullOrEmpty(userName))
            {
                Global.Configuration.User.Account.GetCookieFrom = cookieMode;
                Global.Configuration.User.Account.Cookie = value;
                Global.Configuration.User.Account.Username = userName;
                Functions.Config.Save();
                _config.Cookie = value;
                _config.Username = userName;
                _config.LoggedIn = true;
                result = true;
            }

            return result;
        }

        private string GetUsernameUsingCookie(string value)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, $"JWT={value}");
            var ok = wc.DownloadString("https://api.warframe.market/v1/profile");

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //TODO Modify this method to work with actually grabbing the username- whenever kyc impliments it

            return "TODO";
        }

        private void LB_AccountMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox) sender;

            TextBox_Cookie.Visibility = listBox.SelectedIndex == 3 ? Visibility.Visible : Visibility.Collapsed;

            _config.IsSaveComplete = LB_AccountMode.SelectedIndex == (int) Global.Configuration.User.Account.GetCookieFrom;
        }

        private void Check_recieveMessages_OnClick(object sender, RoutedEventArgs e)
        {
            Global.Configuration.User.Account.GetMessages = _config.RecieveMessages;
            Functions.Config.Save();
        }

        private void Check_matchStatus_OnClick(object sender, RoutedEventArgs e)
        {
            Global.Configuration.User.Account.SetStatus = _config.SetStatus;
            Functions.Config.Save();
        }

        private void Check_toggleWatchers_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            Global.Configuration.Application.Watcher = checkBox.IsChecked.Value;
            Functions.Config.Save();
        }
    }
}
