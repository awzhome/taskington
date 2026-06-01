using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Taskington.Gui.ViewModels;
using System;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ReactiveUI.Avalonia;
using System.Threading.Tasks;
using System.Linq;

namespace Taskington.Gui.Views;

class EditPlanWindow : ReactiveWindow<EditPlanViewModel>
{
    public EditPlanWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.CloseCommand.Subscribe(save => Close(save))));
        this.WhenActivated(d => d(ViewModel!.OpenFolderDialog.RegisterHandler(OpenFolderDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.OpenFileDialog.RegisterHandler(OpenFileDialogAsync)));
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void OpenButtonMenu(object sender, RoutedEventArgs args)
    {
        var senderControl = sender as Control;
        senderControl?.ContextMenu?.Open(senderControl);
    }

    private async Task OpenFolderDialogAsync(IInteractionContext<string?, string?> interaction)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select directory",
                SuggestedStartLocation = interaction.Input is not null
                    ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(interaction.Input)
                    : null
            });

        var selectedFolder = folders.FirstOrDefault()?.Path.AbsolutePath;
        if (selectedFolder is not null)
        {
            interaction.SetOutput(selectedFolder);
        }
    }

    private async Task OpenFileDialogAsync(IInteractionContext<string?, string?> interaction)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select file", AllowMultiple = false, SuggestedFileName = interaction.Input,
        });
            
        var selectedFile = files.FirstOrDefault()?.Path.AbsolutePath;
        if (selectedFile is not null)
        {
            interaction.SetOutput(selectedFile);
        }
    }
}