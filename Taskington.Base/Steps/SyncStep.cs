using Taskington.Base.SystemOperations;

namespace Taskington.Base.Steps
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

        public SyncStep(PlanStep step, Placeholders placeholders)
        {
            PlanStep = step;
            this.placeholders = placeholders;
        }

        public PlanStep PlanStep { get; private set; }

        public SynchronizedObject SynchronizedObject => PlanStep.DefaultProperty switch
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

        public string? From => placeholders.ResolvePlaceholders(PlanStep["from"]);
        public string? To => placeholders.ResolvePlaceholders(PlanStep["to"]);
        public string? File => placeholders.ResolvePlaceholders(PlanStep["name"]);
        public string? Between => placeholders.ResolvePlaceholders(PlanStep["between"]);
        public string? And => placeholders.ResolvePlaceholders(PlanStep["and"]);
    }
}
