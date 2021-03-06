﻿using WebSocketSharp;

namespace WFTDC
{
    public static class Global
    {
        public static Payloads.Status? CurrentExpectedState;
        public static WebSocket ItemWebSocket;
        public static WebSocket WTWebsocket;
        public static C.Configuration Configuration
        {
            get => _Configuration ?? (_Configuration = Functions.Config.Load());

            set => _Configuration = value;
        }

        private static C.Configuration _Configuration { get; set; }
    }
}
