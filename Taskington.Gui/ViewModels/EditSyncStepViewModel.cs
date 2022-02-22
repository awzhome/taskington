using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Gui.Extension;

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

        public Interaction<Unit, string?>? OpenFolderDialogInteraction { get; set; }
        public Interaction<Unit, string?>? OpenFileDialogInteraction { get; set; }

        readonly StepCaptionFragment leftTextPart;
        readonly StepPathFragment fromPathPart;
        readonly StepCaptionFragment middleTextPart;
        readonly StepPathFragment toPathPart;

        public EditSyncStepViewModel(PlanStep step, Placeholders placeholders) : base(step)
        {
            SelectFromCommand = ReactiveCommand.CreateFromTask(OpenSelectFromDialogAsync);
            SelectToCommand = ReactiveCommand.CreateFromTask(OpenSelectToDialogAsync);

            leftTextPart = new();
            fromPathPart = new(placeholders);
            middleTextPart = new();
            toPathPart = new(placeholders);

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
                leftTextPart.Text = $"Synchronize {selectedType?.Caption} from ";
                middleTextPart.Text = " to ";
            }
        }

        public string? from;
        public string? From
        {
            get => from;
            set
            {
                this.RaiseAndSetIfChanged(ref from, value);
                fromPathPart.Text = from;
            }
        }

        public string? to;
        public string? To
        {
            get => to;
            set
            {
                this.RaiseAndSetIfChanged(ref to, value);
                toPathPart.Text = to;
            }
        }

        private void InitializeFromBasicModel(PlanStep step)
        {
            SelectedType = SyncTypes.FirstOrDefault(entry => entry.Type == step.DefaultProperty);
            From = step["from"];
            To = step["to"];

            CaptionFragments = new[] { leftTextPart, fromPathPart, middleTextPart, toPathPart };
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
    }
}
