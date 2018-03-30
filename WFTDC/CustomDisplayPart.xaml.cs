using System.Drawing;
using ToastNotifications.Core;

namespace WFTDC
{
    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        private CustomNotification _customNotification;

        public CustomDisplayPart(CustomNotification customNotification)
        {
            _customNotification = customNotification;
            DataContext = customNotification; // this allows to bind ui with data in notification
            InitializeComponent();
        }
    }
}
