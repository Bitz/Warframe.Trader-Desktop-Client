namespace WFTDC
{
    using System.Windows.Media;

    public static class Constants
    {
        private static readonly string wTSForeground;

        private static readonly string wTSBackground;

        private static readonly string wTBForeground;

        private static readonly string wTBBackground;


        static Constants()
        {
            wTSForeground = Color.FromRgb(127, 44, 98).ToHex();
            wTSBackground = Color.FromRgb(225, 198, 215).ToHex();

            wTBForeground = Color.FromRgb(28, 138, 98).ToHex(); 
            wTBBackground = Color.FromRgb(198, 225, 215).ToHex();
        }

        public static string WTBBackground { get => wTBBackground;  }

        public static string WTBForeground { get => wTBForeground;  }

        public static string WTSBackground { get => wTSBackground;  }

        public static string WTSForeground { get => wTSForeground;  }
    }
}
