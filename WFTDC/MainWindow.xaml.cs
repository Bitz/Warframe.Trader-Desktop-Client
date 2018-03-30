namespace WFTDC
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Mantin.Controls.Wpf.Notification;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ToastPopUp toast = new ToastPopUp("AVSVSDSD", "AAAAAAAAA", NotificationType.Information);
            toast.Background = new LinearGradientBrush(
                Color.FromArgb(255, 4, 253, 82),
                Color.FromArgb(255, 10, 13, 248),
                90);
            toast.Icon = (ImageSource) Resources["pack://application:,,,/Resources/B.png"];
            toast.Show();
        }
    }
}
