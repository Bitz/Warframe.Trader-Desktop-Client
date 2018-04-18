using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using ToastNotifications.Messages;
using WebSocketSharp;

namespace WFTDC.Windows
{
    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon
    {
        private readonly NotifyIcon _notifierIcon = new NotifyIcon();
        private readonly ContextMenu _menu;
        private Timer _singleClickTimer;
        private static System.Timers.Timer aTimer;

        public TrayIcon()
        {
            InitializeComponent();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _menu = new ContextMenu();
            var mConfigure = new MenuItem("Configure");
            _menu.MenuItems.Add(mConfigure);
            mConfigure.Click += (sender, args) => ShowConfigWindow();

            var pauseUnpause = new MenuItem("Pause");
            pauseUnpause.Click += (sender, args) => PauseToggle();
            _menu.MenuItems.Add(pauseUnpause);

            _menu.MenuItems.Add(new MenuItem("-"));

            var mExit = new MenuItem("Exit");
            mExit.Click += (sender, args) => Close();
            _menu.MenuItems.Add(mExit);
            _notifierIcon.Icon = Properties.Resources.wftlogo;
            _notifierIcon.MouseClick += Notifier_MouseClick;
            _notifierIcon.MouseDoubleClick += Notifier_MouseDoubleClick;
            _notifierIcon.Visible = true;
            _notifierIcon.ContextMenu = _menu;

            Global.WebSocket = new WebSocket("ws://ws.bitz.rocks") { Origin = "user://" + Global.Configuration.User.Id };
            //Global.WebSocket = new WebSocket("ws://127.0.0.1:2489") { Origin = "user://" + Global.Configuration.User.Id };
            Global.WebSocket.OnMessage += ReceiveMessage;
            Global.WebSocket.OnClose += WsOnOnClose;
            Global.WebSocket.Compression = CompressionMethod.Deflate;
            Global.WebSocket.Connect();
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += delegate { Global.WebSocket.SendWatchList(); };
            bw.RunWorkerAsync();
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
            if (!closeEventArgs.WasClean && !Global.WebSocket.IsAlive && aTimer == null)
            {
                aTimer = new System.Timers.Timer(10000) { Enabled = true };
                aTimer.Elapsed += Retryconnection;
                aTimer.Start();
                NotificationManager.Notifier.ShowWarning("Connection lost with server, retrying...");
            }
        }

        private void Retryconnection(object sender, ElapsedEventArgs e)
        {
            Global.WebSocket.Connect();
            if (Global.WebSocket.ReadyState == WebSocketState.Open)
            {
                aTimer.Stop();
                NotificationManager.Notifier.ShowSuccess("Connection established!");
                aTimer = null;
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
            PauseToggle();
        }

        private void PauseToggle()
        {
            if (Global.WebSocket.IsAlive)
            {
                Global.WebSocket.Close();
            }
            else
            {
                Global.WebSocket.Connect();
            }

            NotificationManager.Notifier.ShowInformation(Global.WebSocket.IsAlive ? "Trade is now unpaused." : "Trader is now paused.");
            _menu.MenuItems[1].Text = Global.WebSocket.IsAlive ? "Pause" : "Resume";
        }

        private void Notifier_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _singleClickTimer.Stop();
                ShowConfigWindow();
            }
        }

        private MainWindow window;
        private void ShowConfigWindow()
        {
            if (window == null)
            {
                window = new MainWindow();
                window.Closed += (a, b) => window = null;
            }
            window.Show();
            window.Activate();
        }

        private void ReceiveMessage(object sender, MessageEventArgs e)
        {
            string s = Utils.DecompressData(e.RawData);
            PostLoad request = JsonConvert.DeserializeObject<PostLoad>(s);
            
            if (IsDisplayable(request))
            {
                NotificationManager.Notifier.ShowItem(request);
            }
            else
            {
               //NotificationManager.Notifier.ShowItem(request);
            }
        }
        
        private static bool IsDisplayable(PostLoad request)
        {
            // If the item offer is not from our region or platform, we don't care
            if (request.User.Platform != Global.Configuration.User.Platform || request.User.Region != Global.Configuration.User.Region)
            {
                return false;
            }

            // Is the user account in a state that we want? (Ingame, Offline, Online)
            if (!Global.Configuration.User.UserStates.Contains(request.User.Status))
            {
                return false;
            }

            //Do we have any enabled?
            var matcher = Global.Configuration.Items.Where(x => x.Enabled).ToArray();
            if (!matcher.Any())
            {
                return false;
            }

            // Do we have a matching item? (Check the name) 
            matcher = matcher.Where(x => x.UrlName == request.Item.UrlName).ToArray();
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
            if (window != null)
            {
                window.Close();
            }
            Global.WebSocket.Close();
            NotificationManager.Notifier.Dispose();
            _notifierIcon.Dispose();
            Application.Exit();
        }
    }
}
