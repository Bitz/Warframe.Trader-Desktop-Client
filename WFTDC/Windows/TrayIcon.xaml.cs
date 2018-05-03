using System.Diagnostics;

namespace WFTDC.Windows
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Management;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using ToastNotifications.Messages;
    using WebSocketSharp;
    using Payloads.Chat;
    using ChatPayload = Payloads.WebsocketChat.ChatPayload;
    using Payloads;

    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon
    {
        private readonly NotifyIcon _notifierIcon = new NotifyIcon();
        private readonly ContextMenu _menu;
        //private Timer _singleClickTimer;
        private static System.Timers.Timer _aTimer;
        private MainWindow _watcherWindow;
        private Config _configWindow;
        private bool _showedConnectionLostOnce;

        private ManagementEventWatcher _startWatch;
        private ManagementEventWatcher _stopWatch;


        public TrayIcon()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InitializeComponent();

            if (Global.Configuration.Application.StartWithWindows)
            {
                if (!Utils.IsStartUpEnabled())
                {
                    Utils.StartWithWindows();
                }
            }
            else
            {
                Utils.DoNotStartWithWindows();
            }

            _menu = CreateTrayIconContextMenu();


            _notifierIcon.Icon = Properties.Resources.wftlogo;
            _notifierIcon.Text = "Warframe Trader " + Application.ProductVersion;
            _notifierIcon.MouseClick += Notifier_MouseClick;
            _notifierIcon.MouseDoubleClick += Notifier_MouseDoubleClick;
            _notifierIcon.ContextMenu = _menu;

            if (Global.Configuration.Application.Watcher)
            {
                CreateItemWebsocket();
            }

            if (Global.Configuration.User.Account.SetStatus || Global.Configuration.User.Account.GetMessages && !string.IsNullOrEmpty(Global.Configuration.User.Account.Cookie))
            {
                CreateChatWebsocket();
            }

            if (Global.Configuration.User.Account.SetStatus &&
                !string.IsNullOrEmpty(Global.Configuration.User.Account.Cookie))
            {
                WatchForWarframeProcess();
            }

            //Only show icon when we are ready to go.
            _notifierIcon.Visible = true;
        }

        private void WatchForWarframeProcess()
        {
            Process[] pname = Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains("warframe") && x.ProcessName != "Warframe Trader Desktop Client").ToArray();
            if (pname.Length > 0)
            {
                StartWatch_EventArrived(null, null);
            }
            else
            {
                StopWatch_EventArrived(null, null);
            }
            _startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName LIKE '%warframe%'"));
            _startWatch.EventArrived += StartWatch_EventArrived;
            _startWatch.Start();
            _stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName LIKE '%warframe%'"));
            _stopWatch.EventArrived += StopWatch_EventArrived;
            _stopWatch.Start();
        }

        private static void StopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (Global.WTWebsocket.IsAlive)
            {
                Global.CurrentExpectedState = Status.Online;
                Global.WTWebsocket.Send(Utils.SetWebsocketStatusString(Global.CurrentExpectedState.Value));
            }
        }

        private static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (Global.WTWebsocket.IsAlive)
            {
                Global.CurrentExpectedState = Status.Ingame;
                Global.WTWebsocket.Send(Utils.SetWebsocketStatusString(Global.CurrentExpectedState.Value));
            }
        }



        private ContextMenu CreateTrayIconContextMenu()
        {
            var menu = new ContextMenu();

            if (Global.Configuration.Application.Watcher)
            {
                var mConfigureWatcher = new MenuItem("Watchers");
                menu.MenuItems.Add(mConfigureWatcher);
                mConfigureWatcher.Click += (sender, args) => ShowWatcherConfigWindow();
            }

            var mConfigure = new MenuItem("Configure");
            menu.MenuItems.Add(mConfigure);
            mConfigure.Click += (sender, args) => ShowConfigWindow();

            if (Global.Configuration.Application.Watcher)
            {
                menu.MenuItems.Add(new MenuItem("-"));
                var pauseUnpause = new MenuItem("Pause");
                pauseUnpause.Click += (sender, args) => PauseToggle();
                menu.MenuItems.Add(pauseUnpause);
            }

            menu.MenuItems.Add(new MenuItem("-"));

            var mExit = new MenuItem("Exit");
            mExit.Click += (sender, args) => Close();
            menu.MenuItems.Add(mExit);
            return menu;
        }
        
        #region Subwindow Management Code
        private void ShowConfigWindow()
        {
            if (_configWindow == null)
            {
                _configWindow = new Config();
                _configWindow.Closed += (a, b) => _configWindow = null;
            }
            _configWindow.Show();
            _configWindow.Activate();
        }

        private void ShowWatcherConfigWindow()
        {
            if (Global.Configuration.Application.Watcher)
            {
                if (_watcherWindow == null)
                {
                    _watcherWindow = new MainWindow();
                    _watcherWindow.Closed += (a, b) => _watcherWindow = null;
                }
                _watcherWindow.Show();
                _watcherWindow.Activate();
            }
        }
        #endregion

        #region Clicks and actions Code
        private void Notifier_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PauseToggle();
                //_singleClickTimer = new Timer { Interval = (int)(SystemInformation.DoubleClickTime * 1.1) };
                //_singleClickTimer.Tick += SingleClickTimer_Tick;
                //_singleClickTimer.Start();
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        //private void SingleClickTimer_Tick(object sender, EventArgs e)
        //{
        //    //_singleClickTimer.Stop();

        //}

        private void Notifier_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //_singleClickTimer.Stop();
                ShowWatcherConfigWindow();
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _notifierIcon.Dispose();
            _watcherWindow?.Close();
            _configWindow?.Close();

            if (Global.ItemWebSocket != null)
            {
                Global.ItemWebSocket.Close();
            }

            if (Global.WTWebsocket != null)
            {
                if (_startWatch != null || _stopWatch != null && Global.WTWebsocket.IsAlive)
                {
                    Global.WTWebsocket.Send(Utils.SetWebsocketStatusString(Status.Offline));
                    Global.CurrentExpectedState = Status.Offline;
                }
                Global.WTWebsocket.Close();
            }

            _startWatch?.Stop();
            _stopWatch?.Stop();

            NotificationManager.Notifier.Dispose();
            Application.Exit();
        }
        #endregion

        #region Shared Websocket Code
        private void WsOnOnClose(object sender, CloseEventArgs closeEventArgs)
        {
            var ws = (WebSocket)sender;
            if (!closeEventArgs.WasClean && !ws.IsAlive && _aTimer == null)
            {
                _aTimer = new System.Timers.Timer(10000) { Enabled = true };
                _aTimer.Elapsed += (o, args) => Retryconnection(ws);
                _aTimer.Start();
                if (!_showedConnectionLostOnce)
                {
                    NotificationManager.Notifier.ShowWarning("Connection lost with server, retrying...");
                    _showedConnectionLostOnce = true;
                }
            }
        }

        private void Retryconnection(WebSocket ws)
        {
            if (ws.ReadyState != WebSocketState.Open)
            {
                ws.Connect();
            }

            if (ws.ReadyState == WebSocketState.Open)
            {
                _aTimer.Stop();
                _aTimer = null;
                if (_showedConnectionLostOnce)
                {
                    NotificationManager.Notifier.ShowSuccess("Connection established!");
                }
                _showedConnectionLostOnce = false;
            }
        }
        #endregion
        
        #region Websocket Chat Code
        private void CreateChatWebsocket()
        {
            Global.WTWebsocket = new WebSocket("wss://warframe.market/socket")
            {
                Origin = "user://" + Global.Configuration.User.Id,
                SslConfiguration = { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 }
            };
            var authCookie = new WebSocketSharp.Net.Cookie("JWT", Global.Configuration.User.Account.Cookie)
            {
                Path = "/",
                Domain = ".warframe.market"
            };
            Global.WTWebsocket.SetCookie(authCookie);
            Global.WTWebsocket.OnMessage += ReceiveWtMessage;
            Global.WTWebsocket.OnClose += WsOnOnClose;
            Global.WTWebsocket.Connect();
        }

        private void ReceiveWtMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            if (e.Data.Contains("\"type\": \"@WS/chats/NEW_MESSAGE\""))
            {
                var request = JsonConvert.DeserializeObject<ChatPayload>(e.Data);

                BackgroundWorker bw = new BackgroundWorker();
                Chat chat = null;
                string otherUserId = request.Payload.MessageFrom;
                bw.DoWork += delegate
                {
                    chat = GetChat(request.Payload.ChatId);
                };
                bw.WorkerReportsProgress = true;
                bw.RunWorkerAsync();

                bw.RunWorkerCompleted += (o, args) =>
                {
                    if (chat != null) NotificationManager.Notifier.ShowChat(chat, otherUserId);
                };
            }

            if (e.Data.Contains("\"type\": \"@WS/USER/SET_STATUS_DONE\""))
            {
                if (Global.Configuration.User.Account.SetStatus)
                {
                    Status thisStatus = Status.Offline;
                    if (e.Data.Contains("invisible"))
                    {
                        thisStatus = Status.Offline;
                    }
                    else if (e.Data.Contains("offline"))
                    {
                        thisStatus = Status.Offline;
                    }
                    else if (e.Data.Contains("ingame"))
                    {
                        thisStatus = Status.Ingame;
                    }
                    else if (e.Data.Contains("online"))
                    {
                        thisStatus = Status.Online;
                    }

                    if (Global.CurrentExpectedState != null && Global.CurrentExpectedState.Value != thisStatus)
                    {
                        Global.WTWebsocket.Send(Utils.SetWebsocketStatusString(Global.CurrentExpectedState.Value));
                    }
                }
            }
        }

        private Chat GetChat(string chatId)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, $"JWT={Global.Configuration.User.Account.Cookie}");
            var ok = wc.DownloadString("https://api.warframe.market/v1/im/chats");
            var chatsObject = JsonConvert.DeserializeObject<Payloads.Chat.ChatPayload>(ok);
            return chatsObject.Payload.Chats.FirstOrDefault(x => x.Id == chatId);
        }
        #endregion
        
        #region Websocket Watcher Code
        private void CreateItemWebsocket()
        {
            Global.ItemWebSocket = new WebSocket("ws://ws.bitz.rocks") { Origin = "user://" + Global.Configuration.User.Id };
            Global.ItemWebSocket.OnMessage += ReceiveItemMessage;
            Global.ItemWebSocket.OnClose += WsOnOnClose;
            Global.ItemWebSocket.Compression = CompressionMethod.Deflate;
            Global.ItemWebSocket.Connect();
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += delegate { Global.ItemWebSocket.SendWatchList(); };
            bw.RunWorkerAsync();
        }

        private void ReceiveItemMessage(object sender, MessageEventArgs e)
        {
            string s = Utils.DecompressData(e.RawData);
            PostLoad request = JsonConvert.DeserializeObject<PostLoad>(s);

            if (IsDisplayable(request))
            {
                NotificationManager.Notifier.ShowItem(request);
            }
        }

        private void PauseToggle()
        {
            if (Global.Configuration.Application.Watcher)
            {
                if (Global.ItemWebSocket.IsAlive)
                {
                    Global.ItemWebSocket.Close();
                }
                else
                {
                    Global.ItemWebSocket.Connect();
                }

                NotificationManager.Notifier.ShowInformation(Global.ItemWebSocket.IsAlive ? "Trade is now unpaused." : "Trader is now paused.");
                _menu.MenuItems[3].Text = Global.ItemWebSocket.IsAlive ? "Pause" : "Resume";
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

        #endregion
    }
}
