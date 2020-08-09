using PPBackup.Base.Model;
using PPBackup.Base.SystemOperations;
using System.IO;
using System.Linq;

namespace PPBackup.Base.Executors
{
    internal class SyncStepExecution : IStepExecution
    {
        private readonly ISystemOperations systemOperations;

        public SyncStepExecution(ISystemOperations systemOperations)
        {
            this.systemOperations = systemOperations;
        }

        public string Type => "sync";

        public void Execute(BackupStep step, Placeholders placeholders, StepExecutionEvents status)
        {
            var syncStep = new SyncStep(step, placeholders);
            if (syncStep.From != null && syncStep.To != null)
            {
                switch (syncStep.SynchronizedObject)
                {
                    case SynchronizedObject.Directory:
                        {
                            SyncDirectory(syncStep.SyncDirection, syncStep.From, syncStep.To, status);
                            break;
                        }
                    case SynchronizedObject.SubDirectories:
                        {
                            var directories = Directory.GetDirectories(syncStep.From);
                            var directoryCount = directories.Count();
                            int dirsFinished = 0;
                            foreach (var dir in directories)
                            {
                                SyncDirectory(syncStep.SyncDirection, dir, Path.Combine(syncStep.To, Path.GetFileName(dir)), status);
                                dirsFinished++;
                                status.Progress(dirsFinished * 100 / directoryCount);
                            }
                            break;
                        }
                    case SynchronizedObject.File:
                        if (syncStep.File != null)
                        {
                            status.StatusText($"Sync file '{Path.GetFileName(syncStep.File)}'");
                            systemOperations.SyncFile(syncStep.From, syncStep.To, syncStep.File);
                        }
                        break;
                }
            }
        }

        private void SyncDirectory(SyncDirection syncDirection, string dir1, string dir2, StepExecutionEvents status)
        {
            status.StatusText($"Sync directory '{Path.GetFileName(dir1)}'");
            systemOperations.SyncDirectory(syncDirection, dir1, dir2);
        }
    }
}
