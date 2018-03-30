using ToastNotifications;

namespace WFTDC
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public static class Extensions
    {

        public static void ShowItemMessage(this Notifier notifier, string message, string image)
        {
            notifier.Notify<CustomNotification>(() => new CustomNotification(message, image));
        }
    }
}
