using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Taskington.Base.Events;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Steps
{
    internal class SyncStepExecution
    {
        private readonly IEventBus eventBus;

        public SyncStepExecution(IEventBus eventBus)
        {
            this.eventBus = eventBus;

            eventBus.Subscribe<ExecuteStep>(Execute);
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

        public bool CanExecuteSupportedSteps(IEnumerable<PlanStep> steps, Placeholders placeholders)
        {
            return !steps
                .Where(step => step.StepType == "sync")
                .SelectMany(GetRelevantPathsOfStep)
                .SelectMany(placeholders.ExtractPlaceholders)
                .Any(result => result.Placeholder.StartsWith("drive:") && result.Resolved == null);
        }

        public void Execute(ExecuteStep e)
        {
            if (e.Step.StepType == "sync")
            {
                (PlanStep step, Placeholders placeholders, Action<int>? progressCallback, Action<string>? statusTextCallback) = e;
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
                                eventBus.Push(new SyncFile(syncStep.From, syncStep.To, syncStep.File));
                            }
                            break;
                    }
                }
            }
        }

        private void SyncDirectory(SyncDirection syncDirection, string dir1, string dir2, Action<string>? statusTextCallback)
        {
            statusTextCallback?.Invoke($"Sync directory '{Path.GetFileName(dir1)}'");
            eventBus.Push(new SyncDirectory(syncDirection, dir1, dir2));
        }
    }
}
