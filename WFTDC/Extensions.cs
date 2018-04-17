using System.Collections.Generic;

namespace WFTDC
{
    using System.Windows.Media;

    public static partial class Extensions
    {
        public static string ToHex(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static void SendWatchList(this WebSocketSharp.WebSocket ws)
        {
            List<string> itemsToWatch = new List<string>();

            foreach (var item in Global.Configuration.Items)
            {
                itemsToWatch.Add(item.UrlName);
            }

            string itemString = string.Join(",", itemsToWatch);

            var itemCompressed = Utils.CompressData(itemString);
            if (ws.IsAlive)
            {
                ws.Send(itemCompressed);
            }
        }
    }
}
