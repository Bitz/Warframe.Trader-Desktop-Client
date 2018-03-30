namespace WFTDC
{
    using ToastNotifications.Core;

    public class ItemNotification : NotificationBase
    {
        private CustomDisplayPart _displayPart;
        private PostLoad _postLoad;
        private string _iconUrl;
        private string _fullName;
        private string _quantity;
        private string _offerText;
        private string _offerForeground;
        private string _offerBackground;

        public ItemNotification(PostLoad postLoad)
        {
            PostLoad = postLoad;
            IconUrl = "https://warframe.market/static/assets/";

            if (!string.IsNullOrEmpty(_postLoad.Item.SubIcon))
            {
                IconUrl += _postLoad.Item.SubIcon;
            }
            else if (!string.IsNullOrEmpty(_postLoad.Item.Icon))
            {
                IconUrl += _postLoad.Item.Icon;
            }
            else
            {
                IconUrl += "user/default-avatar.png";
            }

            FullName = _postLoad.Item.Name.ItemName;

            Quantity += $"[{postLoad.Quantity}]";

            OfferText = $"{postLoad.Platinum} Platinum";

            if (postLoad.Quantity > 1)
            {
                OfferText += $"Each ({postLoad.Quantity * postLoad.Platinum})";
            }

            switch (_postLoad.Type)
            {
                case OrderType.Buy:
                    _offerForeground = Constants.WTBForeground;
                    _offerBackground = Constants.WTBBackground;
                    break;
                case OrderType.Sell:
                    _offerForeground = Constants.WTSForeground;
                    _offerBackground = Constants.WTSBackground;
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

        public string IconUrl
        {
            get => _iconUrl;
            set => _iconUrl = value;
        }

        public string FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        public string Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public string OfferText
        {
            get => _offerText;
            set => _offerText = value;
        }

        public string OfferForeground
        {
            get => _offerForeground;
            set => _offerForeground = value;
        }

        public string OfferBackground
        {
            get => _offerBackground;
            set => _offerBackground = value;
        }
    }
}