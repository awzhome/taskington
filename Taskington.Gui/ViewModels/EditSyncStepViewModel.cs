using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    class SyncTypeEntry
    {
        public string? Type { get; set; }
        public string? Caption { get; set; }

    }

    class EditSyncStepViewModel : EditStepViewModelBase
    {
        private const string SyncFileType = "file";
        private const string SyncDirType = "dir";
        private const string SyncSubDirsType = "sub-dirs";

        public ReactiveCommand<Unit, Unit> SelectFromCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectToCommand { get; }

        public Interaction<Unit, string>? OpenFolderDialogInteraction { get; set; }
        public Interaction<Unit, string?>? OpenFileDialogInteraction { get; set; }

        public EditSyncStepViewModel(PlanStep step) : base(step)
        {
            SelectFromCommand = ReactiveCommand.CreateFromTask(OpenSelectFromDialogAsync);
            SelectToCommand = ReactiveCommand.CreateFromTask(OpenSelectToDialogAsync);

            InitializeFromBasicModel(step);
        }

        public List<SyncTypeEntry> SyncTypes { get; } = new()
        {
            new() { Type = SyncFileType, Caption = "file" },
            new() { Type = SyncDirType, Caption = "directory" },
            new() { Type = SyncSubDirsType, Caption = "sub-directories" }
        };

        private SyncTypeEntry? selectedType;
        public SyncTypeEntry? SelectedType
        {
            get => selectedType;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedType, value);
                Icon = SelectedType?.Type switch
                {
                    SyncFileType => "fas fa-copy",
                    SyncDirType => "fas fa-folder-open",
                    SyncSubDirsType => "fas fa-sitemap",
                    _ => ""
                };
                UpdateCaption();
            }
        }

        public string? from;
        public string? From
        {
            get => from;
            set
            {
                this.RaiseAndSetIfChanged(ref from, value);
                UpdateCaption();
            }
        }

        public string? to;
        public string? To
        {
            get => to;
            set
            {
                this.RaiseAndSetIfChanged(ref to, value);
                UpdateCaption();
            }
        }

        private void InitializeFromBasicModel(PlanStep step)
        {
            SelectedType = SyncTypes.FirstOrDefault(entry => entry.Type == step.DefaultProperty);
            From = step["from"];
            To = step["to"];
        }

        public override PlanStep ConvertToStep()
        {
            var step = base.ConvertToStep();
            step["from"] = from;
            step["to"] = to;
            return step;
        }

        private async Task OpenSelectFromDialogAsync()
        {
            if (selectedType?.Type == "file")
            {
                if (OpenFileDialogInteraction != null)
                {
                    var selectedFile = await OpenFileDialogInteraction.Handle(Unit.Default);
                    if (!string.IsNullOrEmpty(selectedFile))
                    {
                        From = selectedFile;
                    }
                }
            }
            else
            {
                if (OpenFolderDialogInteraction != null)
                {
                    var selectedPath = await OpenFolderDialogInteraction.Handle(Unit.Default);
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        From = selectedPath;
                    }
                }
            }
        }

        private async Task OpenSelectToDialogAsync()
        {
            if (OpenFolderDialogInteraction != null)
            {
                var selectedPath = await OpenFolderDialogInteraction.Handle(Unit.Default);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    To = selectedPath;
                }
            }
        }

        private void UpdateCaption()
        {
            Caption = $"Synchronize {selectedType?.Caption} from {from} to {to}";
        }
    }
}
