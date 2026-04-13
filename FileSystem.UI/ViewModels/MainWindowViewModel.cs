using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileSystem.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using FileSystem.UI.Views;

namespace FileSystem.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private FolderElement? _rootFolder;

    [ObservableProperty]
    private FileSystemElement? _selectedElement;

    [ObservableProperty]
    private string _statusMessage = "Добро пожаловать в файловую систему";

    public ReflectionViewModel Reflection { get; } = new();

    public MainWindowViewModel()
    {
        InitializeFileSystem();
    }

    private void InitializeFileSystem()
    {
        var root = new FolderElement("C:", null);

        var docsFolder = new FolderElement("Documents", root);
        var imagesFolder = new FolderElement("Images", root);
        var musicFolder = new FolderElement("Music", root);

        var file1 = new FileElement("report.txt", docsFolder, 1024);
        var file2 = new FileElement("photo.jpg", imagesFolder, 204800);
        var file3 = new FileElement("notes.txt", docsFolder, 512);
        var file4 = new FileElement("song.mp3", musicFolder, 5242880);

        root.Children.Add(docsFolder);
        root.Children.Add(imagesFolder);
        root.Children.Add(musicFolder);

        docsFolder.Children.Add(file1);
        docsFolder.Children.Add(file3);
        imagesFolder.Children.Add(file2);
        musicFolder.Children.Add(file4);

        RootFolder = root;
    }

    [RelayCommand]
    private void CopySelected()
    {
        if (SelectedElement == null || RootFolder == null)
        {
            StatusMessage = "❌ Ничего не выбрано для копирования";
            return;
        }

        var targetFolder = SelectedElement.Parent ?? RootFolder;
        var copy = FileSystemElement.Copy(SelectedElement, targetFolder);
        targetFolder.Children.Add(copy);
        StatusMessage = $"✅ Элемент '{copy.Name}' скопирован в {targetFolder.Name}";
    }

    public async Task<bool> MoveSelectedAsync(Window window)
    {
        if (SelectedElement == null || RootFolder == null)
        {
            StatusMessage = "❌ Ничего не выбрано для перемещения";
            return false;
        }

        if (SelectedElement.Parent == null)
        {
            StatusMessage = "❌ Нельзя перемещать корневую папку";
            return false;
        }

        var targetFolder = await FolderSelectDialog.ShowAsync(window, RootFolder, SelectedElement);

        if (targetFolder == null)
        {
            StatusMessage = "ℹ️ Перемещение отменено";
            return false;
        }

        var (success, message) = FileSystemElement.Move(SelectedElement, targetFolder);
        StatusMessage = message;
        return success;
    }

    [RelayCommand]
    private void DeleteSelected()
    {
        if (SelectedElement == null || SelectedElement.Parent == null)
        {
            StatusMessage = "❌ Нечего удалять или элемент в корне";
            return;
        }

        var name = SelectedElement.Name;
        SelectedElement.Parent.Children.Remove(SelectedElement);
        StatusMessage = $"✅ Элемент '{name}' удалён";
        SelectedElement = null;
    }

}