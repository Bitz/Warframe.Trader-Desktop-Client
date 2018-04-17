using WebSocketSharp;

namespace WFTDC
{
    public static class Global
    {
        public static WebSocket WebSocket;
        public static C.Configuration Configuration
        {
            get
            {
                return _Configuration ?? (_Configuration = Functions.Config.Load());
            }

            set
            {
                _Configuration = value;
            }
        }

        private static C.Configuration _Configuration { get; set; }
    }
}
