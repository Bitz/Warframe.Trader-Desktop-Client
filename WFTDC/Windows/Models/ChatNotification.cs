using System.Text.RegularExpressions;

namespace WFTDC.Windows.Models
{
    using System.Linq;
    using ToastNotifications.Core;
    using Payloads;
    using Payloads.Chat;

    public class ChatNotificationModel : NotificationBase
    {
        private ChatNotification _displayPart;
        public readonly Chat _chatload;

        public ChatNotificationModel(Chat chatload, string otherUserId)
        {
            _chatload = chatload;
            var otherUser = _chatload.ChatWith.FirstOrDefault(x => x.Id == otherUserId);


            var m = _chatload.Messages.FirstOrDefault().MessageMessage;
            m = Regex.Replace(m, @"\r\n?|\n", " ");
            int maxLength = 84;
            Message = m.Length > maxLength ? $"{m.Substring(0, maxLength - 3)}..." : m;

            if (string.IsNullOrEmpty(otherUser.Avatar))
            {
                OnlineIconUrl = "https://warframe.market/static/assets/user/default-avatar.png";
            }
            else
            {
                OnlineIconUrl = $"https://warframe.market/static/assets/{otherUser.Avatar}";
            }

            Username = otherUser.IngameName;
            switch (otherUser.Status)
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
            _displayPart ?? (_displayPart = new ChatNotification(this));

        public string Message { get; set; }

        public string TimeAgo { get; set; }

        public string UserStatus { get; set; }

        public string StatusForeground { get; set; }

        public string OnlineIconUrl { get; set; }

        public string Username { get; set; }
    }
}