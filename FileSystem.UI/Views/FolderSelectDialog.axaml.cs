using Avalonia.Controls;
using Avalonia.Interactivity;
using FileSystem.UI.ViewModels;
using FileSystem.Core.Models;
using System;
using System.Threading.Tasks;


namespace FileSystem.UI.Views;

public partial class FolderSelectDialog : Window
{
    public FolderSelectDialog()
    {
        InitializeComponent();

        OkButton.Click += OkButton_Click;
        CancelButton.Click += CancelButton_Click;
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FolderSelectDialogViewModel viewModel)
        {
            if (viewModel.SelectedFolder != null)
            {
                viewModel.DialogResult = true;
                Close();
            }
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FolderSelectDialogViewModel viewModel)
        {
            viewModel.DialogResult = false;
            Close();
        }
    }

    public static async Task<FolderElement?> ShowAsync(Window parent, FolderElement root, FileSystemElement? sourceElement)
    {
        var dialog = new FolderSelectDialog();
        var viewModel = new FolderSelectDialogViewModel();

        viewModel.LoadFolders(root, sourceElement);
        dialog.DataContext = viewModel;
        dialog.Owner = parent;

        await dialog.ShowDialog(parent);

        if (viewModel.DialogResult && viewModel.SelectedFolder != null)
        {
            return viewModel.SelectedFolder.Folder;
        }

        return null;
    }
}