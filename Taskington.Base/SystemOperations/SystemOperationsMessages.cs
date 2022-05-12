using Taskington.Base.TinyBus;

namespace Taskington.Base.SystemOperations
{
    public static class SystemOperationsMessages
    {
        public static Message<SyncDirection, string, string> SyncDirectory { get; } = new();
        public static Message<string, string, string> SyncFile { get; } = new();
        public static RequestMessage<Placeholders> LoadSystemPlaceholders { get; } = new();
    }
}
