using PPBackup.Base.SystemOperations;

namespace PPBackup.Base.Steps
{
    enum SynchronizedObject
    {
        Directory,
        SubDirectories,
        File
    }

    internal class SyncStep
    {
        private readonly Placeholders placeholders;

        public SyncStep(BackupStep step, Placeholders placeholders)
        {
            BackupStep = step;
            this.placeholders = placeholders;
        }

        public BackupStep BackupStep { get; private set; }

        public SynchronizedObject SynchronizedObject => BackupStep.DefaultProperty switch
        {
            "dir" => SynchronizedObject.Directory,
            "sub-dirs" => SynchronizedObject.SubDirectories,
            _ => SynchronizedObject.File
        };

        public SyncDirection SyncDirection
        {
            get
            {
                if (From != null && To != null)
                {
                    return SyncDirection.FromTo;
                }
                else if (Between != null && And != null)
                {
                    return SyncDirection.Both;
                }

                return SyncDirection.Undefined;
            }
        }

        public string? From => placeholders.ResolvePlaceholders(BackupStep["from"]);
        public string? To => placeholders.ResolvePlaceholders(BackupStep["to"]);
        public string? File => placeholders.ResolvePlaceholders(BackupStep["name"]);
        public string? Between => placeholders.ResolvePlaceholders(BackupStep["between"]);
        public string? And => placeholders.ResolvePlaceholders(BackupStep["and"]);
    }
}
