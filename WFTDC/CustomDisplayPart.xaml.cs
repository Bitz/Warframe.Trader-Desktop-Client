using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace WFTDC
{
    using ToastNotifications.Core;

    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        private readonly ItemNotification _itemNotification;

        public CustomDisplayPart(ItemNotification itemNotification)
        {
            _itemNotification = itemNotification;
            DataContext = itemNotification; // this allows to bind ui with data in notification
            InitializeComponent();
            WTTextBlock.Text = itemNotification.PostLoad.Type == OrderType.Buy ? "WTB" : "WTS";
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            var dc = DataContext as INotification;
            dc.CanClose = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var dc = DataContext as INotification;
            dc.CanClose = true;
        }

        private void UserNameTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://warframe.market/profile/" + _itemNotification.PostLoad.User.Name);
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var w = Application.Current.Windows[0];
            w.Hide();
        }
    }
}
