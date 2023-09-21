using DynamicData;
using System.Collections.ObjectModel;
using System.Linq;
using Taskington.Gui.Extension;

namespace Taskington.Gui.ViewModels;

internal interface IAppNotificationViewModel : IAppNotifications
{
    ObservableCollection<AppNotification> Notifications { get; }
}

internal class AppNotificationViewModel : IAppNotificationViewModel
{
    public ObservableCollection<AppNotification> Notifications { get; } = new();

    public void Add(AppNotification notification)
    {
        var notifications = Notifications.ToList();
        notifications.Add(notification);
        var sortedNotifications = notifications.OrderByDescending(notification => notification.NotificationType);
        Notifications.Clear();
        Notifications.AddRange(sortedNotifications);
    }

    public void Remove(AppNotification notification) => Notifications.Remove(notification);
}