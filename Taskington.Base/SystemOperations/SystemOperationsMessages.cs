using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.SystemOperations
{
    public static class SystemOperationsMessages
    {
        public static MessageEndPoint<SyncDirection, string, string> SyncDirectory { get; } = new();
        public static MessageEndPoint<string, string, string> SyncFile { get; } = new();
        public static RequestMessageEndPoint<Placeholders> LoadSystemPlaceholders { get; } = new();
    }
}
