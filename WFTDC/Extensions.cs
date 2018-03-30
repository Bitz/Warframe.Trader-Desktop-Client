namespace WFTDC
{
    using System.Windows.Media;

    public static partial class Extensions
    {
        public static string ToHex(this Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
