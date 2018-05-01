using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ToastNotifications.Core;
using WFTDC.Windows.Models;

namespace WFTDC.Windows
{
    public partial class ChatNotification
    {
        private readonly ChatNotificationModel _chatNotification;

        public ChatNotification(ChatNotificationModel itemNotification)
        {
            _chatNotification = itemNotification;
            DataContext = itemNotification; // this allows to bind ui with data in notification
            InitializeComponent();
            Image.MouseUp += ClickImage;
        }

        private void ClickImage(object sender, MouseButtonEventArgs e)
        {
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (DataContext is INotification dc) dc.CanClose = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (DataContext is INotification dc) dc.CanClose = true;
        }

        private void UserNameTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start($"https://warframe.market/profile/{_chatNotification.Username}");
        }

        private void UIElement_OnMouseUp(object sender, RoutedEventArgs routedEventArgs)
        {
            if (DataContext is INotification dc) dc.Close();
        }

        private void OpenChat_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start($"https://warframe.market/im/chats/{_chatNotification._chatload.Id}");
        }
    }
}
