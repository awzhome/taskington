using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Taskington.Base.Plans;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Steps
{
    internal class SyncStepExecution
    {
        public SyncStepExecution()
        {
            PlanEvents.ExecuteStep.Subscribe(Execute,
                (PlanStep step, Placeholders placeholders, Action<int> progressCallback, Action<string> statusTextCallback) => step.StepType == "sync");
            PlanEvents.PreCheckPlanExecution.Subscribe(PreCheckPlanExecution);
        }

        private IEnumerable<string> GetRelevantPathsOfStep(PlanStep step)
        {
            var from = step["from"];
            if (from != null)
            {
                yield return from;
            }
            var to = step["to"];
            if (to != null)
            {
                yield return to;
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public void PreCheckPlanExecution(Plan plan)
        {
#if SYS_OPS_DRYRUN
            var canExecute = true;
#else
            var placeholders = SystemOperationsEvents.LoadSystemPlaceholders.Request().First();

            var canExecute = !plan.Steps
                .Where(step => step.StepType == "sync")
                .SelectMany(GetRelevantPathsOfStep)
                .SelectMany(placeholders.ExtractPlaceholders)
                .Any(result => result.Placeholder.StartsWith("drive:") && result.Resolved == null);

#endif
            PlanEvents.PlanCanExecuteUpdated.Push(plan, canExecute);
        }
#pragma warning restore CA1822 // Mark members as static

        public void Execute(PlanStep step, Placeholders placeholders, Action<int> progressCallback, Action<string> statusTextCallback)
        {
            var syncStep = new SyncStep(step, placeholders);
            if (syncStep.From != null && syncStep.To != null)
            {
                switch (syncStep.SynchronizedObject)
                {
                    case SynchronizedObject.Directory:
                        {
                            SyncDirectory(syncStep.SyncDirection, syncStep.From, syncStep.To, statusTextCallback);
                            break;
                        }
                    case SynchronizedObject.SubDirectories:
                        {
                            var directories = Directory.GetDirectories(syncStep.From);
                            var directoryCount = directories.Length;
                            int dirsFinished = 0;
                            foreach (var dir in directories)
                            {
                                SyncDirectory(syncStep.SyncDirection, dir, Path.Combine(syncStep.To, Path.GetFileName(dir)), statusTextCallback);
                                dirsFinished++;
                                progressCallback?.Invoke(dirsFinished * 100 / directoryCount);
                            }
                            break;
                        }
                    case SynchronizedObject.File:
                        if (syncStep.File != null)
                        {
                            statusTextCallback?.Invoke($"Sync file '{Path.GetFileName(syncStep.File)}'");
                            SystemOperationsEvents.SyncFile.Push(syncStep.From, syncStep.To, syncStep.File);
                        }
                        break;
                }
            }
        }

        private static void SyncDirectory(SyncDirection syncDirection, string dir1, string dir2, Action<string>? statusTextCallback)
        {
            statusTextCallback?.Invoke($"Sync directory '{Path.GetFileName(dir1)}'");
            SystemOperationsEvents.SyncDirectory.Push(syncDirection, dir1, dir2);
        }
    }
}
