using System.IO;
using ToastNotifications.Core;

namespace WFTDC.Windows.Models
{
    public class ItemNotification : NotificationBase
    {
        private CustomDisplayPart _displayPart;
        private PostLoad _postLoad;

        public ItemNotification(PostLoad postLoad)
        {
            PostLoad = postLoad;

            OnlineIconUrl = Path.Combine(Functions.PathToTemp(), Path.GetFileName(FNA.GetFilePair(postLoad.Item.UrlName).FileName));

            FullName = _postLoad.Item.Name;

            Quantity += $"[{postLoad.Quantity}]";

            OfferText = $"{postLoad.Platinum} Platinum";

            if (postLoad.Quantity > 1)
            {
                OfferText += $" Each ({postLoad.Quantity * postLoad.Platinum})";
            }

            switch (_postLoad.Type)
            {
                case OrderType.Buy:
                    OfferForeground = Constants.WtbForeground;
                    OfferBackground = Constants.WtbBackground;
                    break;
                case OrderType.Sell:
                    OfferForeground = Constants.WtsForeground;
                    OfferBackground = Constants.WtsBackground;
                    break;
            }

            switch (_postLoad.User.Status)
            {
                case Status.Ingame:
                    UserStatus = "ONLINE IN GAME";
                    StatusForeground = Constants.StatusForegroundIngame;
                    break;
                case Status.Offline:
                    UserStatus = "OFFLINE";
                    StatusForeground = Constants.StatusForegroundOffline;
                    break;
                case Status.Online:
                    UserStatus = "ONLINE";
                    StatusForeground = Constants.StatusForegroundOnline;
                    break;
            }
        }

        public override NotificationDisplayPart DisplayPart =>
            _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public PostLoad PostLoad
        {
            get => _postLoad;
            set => _postLoad = value;
        }

        public string OnlineIconUrl { get; set; }

        public string FullName { get; set; }

        public string Quantity { get; set; }

        public string OfferText { get; set; }

        public string OfferForeground { get; set; }

        public string OfferBackground { get; set; }

        public string UserStatus { get; set; }

        public string StatusForeground { get; set; }
    }
}