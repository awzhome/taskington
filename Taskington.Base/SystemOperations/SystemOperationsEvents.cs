using Taskington.Base.TinyBus;

namespace Taskington.Base.SystemOperations
{
    public static class SystemOperationsEvents
    {
        public static Event<SyncDirection, string, string> SyncDirectory { get; } = new();
        public static Event<string, string, string> SyncFile { get; } = new();
        public static RequestEvent<Placeholders> LoadSystemPlaceholders { get; } = new();
    }
}
