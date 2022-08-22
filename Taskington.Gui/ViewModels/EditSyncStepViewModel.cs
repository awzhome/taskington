using ReactiveUI;
using System.Collections.Generic;
using System.Globalization;
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
        public ReactiveCommand<Unit, Unit> PutPlaceholderInFromFieldCommand { get; }
        public ReactiveCommand<Unit, Unit> PutPlaceholderInToFieldCommand { get; }

        public Interaction<string?, string?>? OpenFolderDialogInteraction { get; set; }
        public Interaction<string?, string?>? OpenFileDialogInteraction { get; set; }

        readonly StepCaptionFragment leftTextPart;
        readonly StepPathFragment fromPathPart;
        readonly StepCaptionFragment middleTextPart;
        readonly StepPathFragment toPathPart;
        private readonly Placeholders placeholders;

        public EditSyncStepViewModel(PlanStep step, Placeholders placeholders) : base(step)
        {

            SelectFromCommand = ReactiveCommand.CreateFromTask(OpenSelectFromDialogAsync);
            SelectToCommand = ReactiveCommand.CreateFromTask(OpenSelectToDialogAsync);
            PutPlaceholderInFromFieldCommand = ReactiveCommand.Create(PutPlaceholderInFromField);
            PutPlaceholderInToFieldCommand = ReactiveCommand.Create(PutPlaceholderInToField);

            leftTextPart = new();
            fromPathPart = new(placeholders);
            middleTextPart = new();
            toPathPart = new(placeholders);

            this.placeholders = placeholders;
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
                SubType = selectedType?.Type;
                Icon = selectedType?.Type switch
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
                ShowPutPlaceholderIntoFromButton = ContainsPlaceholderValues(value);
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
                ShowPutPlaceholderIntoToButton = ContainsPlaceholderValues(value);
            }
        }

        public bool? showPutPlaceholderIntoFromButton = false;
        public bool? ShowPutPlaceholderIntoFromButton
        {
            get => showPutPlaceholderIntoFromButton;
            set
            {
                this.RaiseAndSetIfChanged(ref showPutPlaceholderIntoFromButton, value);
            }
        }

        public bool? showPutPlaceholderIntoToButton = true;
        public bool? ShowPutPlaceholderIntoToButton
        {
            get => showPutPlaceholderIntoToButton;
            set
            {
                this.RaiseAndSetIfChanged(ref showPutPlaceholderIntoToButton, value);
            }
        }

        public string PutPlaceholdersButtonTooltip { get; } =
            "Your path contains parts which can be replaced with placeholders.\nThis improves portability and stability of your plan.";

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
                    var selectedFile = await OpenFileDialogInteraction.Handle(from);
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
                    var selectedPath = await OpenFolderDialogInteraction.Handle(from);
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
                var selectedPath = await OpenFolderDialogInteraction.Handle(to);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    To = selectedPath;
                }
            }
        }

        private void PutPlaceholderInFromField()
        {
            From = ReplaceWithPlaceholderValues(From);
        }

        private void PutPlaceholderInToField()
        {
            To = ReplaceWithPlaceholderValues(To);
        }

        private bool ContainsPlaceholderValues(string? path)
        {
            return path switch
            {
                not null => placeholders.Values.Any(val => path.StartsWith(val, true, CultureInfo.InvariantCulture)),
                _ => false
            };
        }

        private string? ReplaceWithPlaceholderValues(string? path)
        {
            if (path != null)
            {
                KeyValuePair<string, string>? placeholderInPath =
                    placeholders.Entries.FirstOrDefault(keyValue => path.StartsWith(keyValue.Value, true, CultureInfo.InvariantCulture));
                if (placeholderInPath != null)
                {
                    return path.Replace(placeholderInPath.Value.Value, $"${{{placeholderInPath.Value.Key}}}", true, CultureInfo.InvariantCulture);
                }
            }

            return path;
        }
    }
}
