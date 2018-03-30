using ToastNotifications.Core;

namespace WFTDC
{
    using System;

    using ToastNotifications;
    using ToastNotifications.Lifetime;
    using ToastNotifications.Position;

    public static partial class Extensions
    {
        public static void ShowItem(this Notifier notifier, PostLoad postLoad)
        {
            notifier.Notify<ItemNotification>(() => new ItemNotification(postLoad));
        }
    }

    public class NotificationManager
    {
        private static Notifier _Notifier { get; set; }

        private static void CloseAction(NotificationBase obj)
        {
            var opts = obj.DisplayPart.GetOptions();
        }

        public static Notifier Notifier
        {
            get
            {
                if (_Notifier == null)
                {
                    int offSet = 0;
                    if (Utils.GetTaskBarLocation() == Utils.TaskBarLocation.Bottom)
                    {
                        offSet = Utils.GetTaskBarHeight() + 8;
                    }

                    _Notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new PrimaryScreenPositionProvider(Corner.BottomRight, 8, offSet);
                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(5));
                        cfg.DisplayOptions.Width = 400;
                    });
                }

                return _Notifier;
            }

            set
            {
                _Notifier = value;
            }
        }
    }
}
