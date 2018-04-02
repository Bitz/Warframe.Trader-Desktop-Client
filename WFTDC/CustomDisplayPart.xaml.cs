using System.Windows;
using static System.Diagnostics.Process;

namespace WFTDC
{
    using System.Windows.Input;
    using ToastNotifications.Core;

    public partial class CustomDisplayPart
    {
        private readonly ItemNotification _itemNotification;

        public CustomDisplayPart(ItemNotification itemNotification)
        {
            _itemNotification = itemNotification;
            DataContext = itemNotification; // this allows to bind ui with data in notification
            InitializeComponent();
            WtTextBlock.Text = itemNotification.PostLoad.Type == OrderType.Buy ? "WTB" : "WTS";
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
            Start("https://warframe.market/profile/" + _itemNotification.PostLoad.User.Name);
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is INotification dc) dc.Close();
        }

        private void AddMessageToClipboard_OnClick(object sender, RoutedEventArgs e)
        {
            string message =
                $"/w {_itemNotification.PostLoad.User.Name} Hi! I want to {_itemNotification.PostLoad.Type.ToString().ToLower()}: {_itemNotification.PostLoad.Item.Name.ItemName} for {_itemNotification.PostLoad.Platinum} platinum. (warframe.market)";
            Clipboard.SetText(message);
            if (DataContext is INotification dc) dc.Close();
        }

        private void SendMessageOnWebsite_OnClick(object sender, RoutedEventArgs e)
        {
            Start("https://warframe.market/im/chats");
        }
    }
}
