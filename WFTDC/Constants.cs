using System.Collections.Generic;
using System.IO;
using WFTDC.Items;

namespace WFTDC
{
    using System.Windows.Media;

    public class Constants
    {
        static Constants()
        {
            WtsForeground = Color.FromRgb(127, 44, 98).ToHex();
            WtsBackground = Color.FromRgb(225, 198, 215).ToHex();

            WtbForeground = Color.FromRgb(28, 138, 98).ToHex(); 
            WtbBackground = Color.FromRgb(198, 225, 215).ToHex();

            StatusForegroundOnline = Colors.DarkGreen.ToHex();
            StatusForegroundIngame = Colors.MediumPurple.ToHex();
            StatusForegroundOffline = Colors.DarkRed.ToHex();

            ApplicationPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string WtbBackground { get; }

        public static List<Items.En> ItemDatabase {
            get { return _ItemDatabase ?? (_ItemDatabase = Functions.Data.GetItemsDatabase()); }
            set { _ItemDatabase = value; }
        }

        private static List<Items.En> _ItemDatabase { get; set; }

        public static FNA.Model FillNameDatabase
        {
            get { return _FillNameDatabase ?? (_FillNameDatabase = FNA.LoadDB()); }
            set { _FillNameDatabase = value; }
        }

        private static FNA.Model _FillNameDatabase { get; set; }

        public static string WtbForeground { get; }

        public static string WtsBackground { get; }

        public static string WtsForeground { get; }

        public static string StatusForegroundOnline { get; }

        public static string StatusForegroundIngame { get; }

        public static string StatusForegroundOffline { get; }

        public static string ApplicationPath;
    }
}
