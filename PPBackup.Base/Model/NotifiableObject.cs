using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PPBackup.Base.Model
{
    public abstract class NotifiableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
