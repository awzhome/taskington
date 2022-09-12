using Taskington.Base.TinyBus;

namespace Taskington.Base.SystemOperations;

public record SyncDirectoryMessage(SyncDirection Direction, string From, string To) : Message<SyncDirectoryMessage>;
public record SyncFileMessage(string From, string To, string FileName) : Message<SyncFileMessage>;
public record LoadSystemPlaceholdersMessage : RequestMessage<Placeholders>;
