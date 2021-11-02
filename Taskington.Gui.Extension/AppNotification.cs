using Taskington.Base.Model;

namespace Taskington.Gui.Extension
{
    public enum AppNotificationType
    {
        AppInfo,
        Info,
        Warning
    }

    public class AppNotification : NotifiableObject
    {
        private string? leftText;

        public string? LeftText
        {
            get => leftText;
            set => SetAndNotify(ref leftText, value);
        }

        private string? rightText;

        public string? RightText
        {
            get => rightText;
            set => SetAndNotify(ref rightText, value);
        }

        private AppNotificationType notificationType;

        public AppNotificationType NotificationType
        {
            get => notificationType;
            set => SetAndNotify(ref notificationType, value);
        }
    }
}
