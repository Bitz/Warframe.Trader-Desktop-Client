


using System.Threading;

namespace WFTDC
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using static System.Windows.Application;

    using Newtonsoft.Json;
    using WebSocketSharp;

    using WFMSocketizer;

    using ToastNotifications;
    using ToastNotifications.Lifetime;
    using ToastNotifications.Position;

    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon
    {
        private readonly WebSocket _ws;
        private readonly NotifyIcon _notifierIcon = new NotifyIcon();
        private Timer _singleClickTimer;

        public TrayIcon()
        {
            _ws = new WebSocket("ws://ws.bitz.rocks") { Origin = "user://" + Global.Configuration.User.Id };
            InitializeComponent();
            _notifierIcon.Icon = Properties.Resources.TrayIcon;
            _notifierIcon.MouseClick += Notifier_MouseClick;
            _notifierIcon.MouseDoubleClick += Notifier_MouseDoubleClick;
            _notifierIcon.Visible = true;

            int offSet = 0;
            if (Utils.GetTaskBarLocation() == Utils.TaskBarLocation.Bottom)
            {
                offSet = Utils.GetTaskBarHeight();
            }
            Notifier notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 8, offSet);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(3),
                    MaximumNotificationCount.FromCount(5));
                cfg.DisplayOptions.Width = 400;
                cfg.Dispatcher = Current.Dispatcher;
            });

            notifier.ShowItemMessage("An item you wanted is now on sale", "https://warframe.market/static/assets/icons/en/Primed_Continuity.240ece0417bbfeff4410681ea2294f13.png");
            Thread.Sleep(1000);
            notifier.ShowItemMessage("An item you wanted is now on sale", "https://warframe.market/static/assets/icons/en/Primed_Continuity.240ece0417bbfeff4410681ea2294f13.png");
            Thread.Sleep(1000);
            notifier.ShowItemMessage("An item you wanted is now on sale", "https://warframe.market/static/assets/icons/en/Primed_Continuity.240ece0417bbfeff4410681ea2294f13.png");

            //_ws.OnMessage += ReceiveMessage;
            _ws.Connect();
            if (Global.Configuration.User.Account.Enabled && string.IsNullOrEmpty(Global.Configuration.User.Account.Cookie))
            {
                string cookie = string.Empty;
                switch (Global.Configuration.User.Account.GetCookieFrom)
                {
                    case Account.GetCookieFromEnum.ManualEntry:
                        break;
                    case Account.GetCookieFromEnum.Chrome:
                        Cookie.GetCookieFromChrome("warframe.market", "JWT", ref cookie);
                        Global.Configuration.User.Account.Cookie = cookie;
                        Functions.Config.Save();
                        break;
                    case Account.GetCookieFromEnum.InternetExplorer:
                        Cookie.GetCookieFromInternetExplorer("warframe.market", "JWT", ref cookie);
                        Global.Configuration.User.Account.Cookie = cookie;
                        Functions.Config.Save();
                        break;
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
        }

        private void SingleClickTimer_Tick(object sender, EventArgs e)
        {
            _singleClickTimer.Stop();

            if (_ws.IsAlive)
            {
                _notifierIcon.ShowBalloonTip(
                    1000,
                    "Warframe Trader Paused",
                    "No offers will be received while paused.",
                    ToolTipIcon.Info);
                _ws.Close();
            }
            else
            {
                _notifierIcon.ShowBalloonTip(
                    1000,
                    "Warframe Trader Unpaused",
                    "Back to work!",
                    ToolTipIcon.Info);
                _ws.Connect();
            }
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

            if (IsDisplayable(request))
            {
                _notifierIcon.ShowBalloonTip(
                    1000,
                    "Warframe Trader",
                    "Someone is selling what you want at a price you want!",
                    ToolTipIcon.Info);
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
                (request.Platinum >= x.Price && x.Type == OrderType.Sell) || // (Equal to or cheaper than our buy offer)
                (request.Platinum <= x.Price && x.Type == OrderType.Buy)) // (Equal or better than our sell offer)
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
    }
}
