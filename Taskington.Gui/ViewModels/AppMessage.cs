using Taskington.Base.Model;

namespace Taskington.Gui.ViewModels
{
    public enum AppMessageType
    {
        AppInfo,
        Info,
        Warning
    }

    public class AppMessage : NotifiableObject
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

        private AppMessageType messageType;

        public AppMessageType MessageType
        {
            get => messageType;
            set => SetAndNotify(ref messageType, value);
        }
    }
}
