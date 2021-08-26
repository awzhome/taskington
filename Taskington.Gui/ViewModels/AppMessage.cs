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
        private string? text;

        public string? Text
        {
            get => text;
            set
            {
                text = value;
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
