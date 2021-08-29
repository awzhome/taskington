using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Taskington.Base.Model
{
    public abstract class NotifiableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChange([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetAndNotify<T>(ref T member, T newValue, [CallerMemberName] string propertyName = "")
        {
            member = newValue;
            NotifyPropertyChange(propertyName);
        }
    }
}
