

using System.Windows;

namespace WFTDC
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Threading;

    using Newtonsoft.Json;
    
    using ToastNotifications.Messages;

    using WebSocketSharp;

    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon
    {
        private readonly WebSocket _ws;
        private readonly NotifyIcon _notifierIcon = new NotifyIcon();
        private readonly ContextMenu _menu;
        private Timer _singleClickTimer;

        public TrayIcon()
        {
            InitializeComponent();

            _menu = new ContextMenu();
            var mConfigure = new MenuItem("Configure");
            _menu.MenuItems.Add(mConfigure);

            _menu.MenuItems.Add(new MenuItem("Pause"));

            _menu.MenuItems.Add(new MenuItem("-"));

            var mExit = new MenuItem("Exit");
            mExit.Click += (sender, args) => Close();
            _menu.MenuItems.Add(mExit);
            _notifierIcon.Icon = Properties.Resources.TrayIcon;
            _notifierIcon.MouseClick += Notifier_MouseClick;
            _notifierIcon.MouseDoubleClick += Notifier_MouseDoubleClick;
            _notifierIcon.Visible = true;

            _notifierIcon.ContextMenu = _menu;
            _ws = new WebSocket("ws://ws.bitz.rocks") { Origin = "user://" + Global.Configuration.User.Id };
            _ws.OnMessage += ReceiveMessage;
            _ws.OnClose += WsOnOnClose;
            _ws.Connect();
            ////if (Global.Configuration.User.Account.Enabled && string.IsNullOrEmpty(Global.Configuration.User.Account.Cookie))
            ////{
            ////    string cookie = string.Empty;
            ////    switch (Global.Configuration.User.Account.GetCookieFrom)
            ////    {
            ////        case Account.GetCookieFromEnum.ManualEntry:
            ////            break;
            ////        case Account.GetCookieFromEnum.Chrome:
            ////            Cookie.GetCookieFromChrome("warframe.market", "JWT", ref cookie);
            ////            Global.Configuration.User.Account.Cookie = cookie;
            ////            Functions.Config.Save();
            ////            break;
            ////        case Account.GetCookieFromEnum.InternetExplorer:
            ////            Cookie.GetCookieFromInternetExplorer("warframe.market", "JWT", ref cookie);
            ////            Global.Configuration.User.Account.Cookie = cookie;
            ////            Functions.Config.Save();
            ////            break;
            ////    }
            ////}
        }

        private void WsOnOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            if (!closeEventArgs.WasClean)
            {
                if (!_ws.IsAlive)
                {
                    Thread.Sleep(10000);
                    _ws.Connect();
                }
            }
        }

        private void Notifier_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _singleClickTimer = new Timer { Interval = (int)(SystemInformation.DoubleClickTime * 1.1) };
                _singleClickTimer.Tick += SingleClickTimer_Tick;
                _singleClickTimer.Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void SingleClickTimer_Tick(object sender, EventArgs e)
        {
            _singleClickTimer.Stop();

            if (_ws.IsAlive)
            {
                _ws.Close();
            }
            else
            {
                _ws.Connect();
            }

            NotificationManager.Notifier.ShowInformation(_ws.IsAlive ? "Trade is now unpaused." : "Trader is now paused.");
        }

        private void Notifier_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _singleClickTimer.Stop();
                MainWindow window = new MainWindow();
                window.Show();
                window.Activate();
            }
        }

        private void ReceiveMessage(object sender, MessageEventArgs e)
        {
            string s = Utils.DecompressData(e.RawData);
            PostLoad request = JsonConvert.DeserializeObject<PostLoad>(s);

            //NotificationManager.Notifier.ShowItem(request);
            if (IsDisplayable(request))
            {
                NotificationManager.Notifier.ShowItem(request);
            }
        }
        
        // ReSharper disable once StyleCop.SA1204
        private static bool IsDisplayable(PostLoad request)
        {
            // If the item offer is not from our region or platform, we don't care
            if (request.User.Platform != Global.Configuration.User.Platform || request.User.Region != Global.Configuration.User.Region)
            {
                return false;
            }

            // Do we have a matching item? (Check the name)
            var matcher = Global.Configuration.Items.Where(x => x.Name == request.Item.UrlName).ToArray();
            if (!matcher.Any())
            {
                return false;
            }

            // Do we have a matching type? (Buy or sell)
            matcher = matcher.Where(x => x.Type == request.Type).ToArray();
            if (!matcher.Any())
            {
                return false;
            }

            // Do we have a matching price? 
            matcher = matcher.Where(x =>
                (request.Platinum <= x.Price && x.Type == OrderType.Sell) || // (Equal to or cheaper than our buy offer)
                (request.Platinum >= x.Price && x.Type == OrderType.Buy)) // (Equal or better than our sell offer)
                .ToArray(); 
            if (!matcher.Any())
            {
                return false;
            }

            // Do we have a matching quantity? (More than our min, but less than our max)
            matcher = matcher.Where(x => request.Quantity >= x.QuantityMin && request.Quantity <= x.QuantityMax).ToArray();
            if (!matcher.Any())
            {
                return false;
            }

            // Is the user account in a state that we want? (Ingame, Offline, Online)
            matcher = matcher.Where(x => x.UserStates.Contains(request.User.Status)).ToArray();
            if (!matcher.Any())
            {
                return false;
            }

            // Is this the sort of item that has ranks? If so, we need to compare ranks
            if (request.ModRank.HasValue)
            {
                // Do we have a matching rank? (More than our min, but less than our max)
                matcher = matcher.Where(x => request.ModRank.Value >= x.ModRankMin && request.ModRank.Value <= x.ModRankMax).ToArray();
                if (!matcher.Any())
                {
                    return false;
                }
            }

            return matcher.Any();
        }

        public void OnWindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _ws.Close();
            NotificationManager.Notifier.Dispose();
            _notifierIcon.Dispose();
        }

        private void CleanExit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
