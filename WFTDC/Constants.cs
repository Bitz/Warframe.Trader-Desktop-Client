namespace WFTDC
{
    using System.Windows.Media;
    using System.IO;

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
