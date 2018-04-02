namespace WFTDC
{
    using System.Windows.Media;

    public static class Constants
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
        }

        public static string WtbBackground { get; }

        public static string WtbForeground { get; }

        public static string WtsBackground { get; }

        public static string WtsForeground { get; }

        public static string StatusForegroundOnline { get; }

        public static string StatusForegroundIngame { get; }

        public static string StatusForegroundOffline { get; }
    }
}
