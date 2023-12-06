namespace Taskington.Gui.Extension;

public interface IAppNotifications
{
    void Add(AppNotification notification);
    void Remove(AppNotification notification);
}