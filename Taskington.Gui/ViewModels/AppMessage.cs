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
            set
            {
                leftText = value;
                NotifyPropertyChange();
            }
        }

        private string? rightText;

        public string? RightText
        {
            get => rightText;
            set
            {
                rightText = value;
                NotifyPropertyChange();
            }
        }

        private AppMessageType messageType;

        public AppMessageType MessageType
        {
            get => messageType;
            set
            {
                messageType = value;
                NotifyPropertyChange();
            }
        }
    }
}
