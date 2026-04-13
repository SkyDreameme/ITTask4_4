using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileSystem.Core.Models;
using System.Collections.ObjectModel;

namespace FileSystem.UI.ViewModels;

public partial class FolderSelectDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private FolderNode? _selectedFolder;

    [ObservableProperty]
    private ObservableCollection<FolderNode> _availableFolders;

    public FileSystemElement? SourceElement { get; set; }
    public bool DialogResult { get; set; } = false;

    public FolderSelectDialogViewModel()
    {
        _availableFolders = new ObservableCollection<FolderNode>();
    }

    public void LoadFolders(FolderElement root, FileSystemElement? sourceElement)
    {
        SourceElement = sourceElement;

        if (root != null)
        {
            var rootNode = new FolderNode(root, false, new ObservableCollection<FolderNode>());

            foreach (var child in root.Children)
            {
                if (child is FolderElement childFolder)
                {
                    var childNode = CreateFolderNode(childFolder, true);
                    rootNode.Children.Add(childNode);
                }
            }

            AvailableFolders.Add(rootNode);
        }
    }

    private FolderNode CreateFolderNode(FolderElement folder, bool canExpand)
    {
        if (SourceElement is FolderElement sourceFolder && sourceFolder == folder)
            return new FolderNode(folder, false, new ObservableCollection<FolderNode>());

        if (SourceElement is FolderElement && IsDescendantOf(folder, SourceElement))
            return new FolderNode(folder, false, new ObservableCollection<FolderNode>());

        var children = new ObservableCollection<FolderNode>();
        var node = new FolderNode(folder, canExpand, children);

        if (canExpand)
        {
            foreach (var child in folder.Children)
            {
                if (child is FolderElement childFolder)
                {
                    children.Add(CreateFolderNode(childFolder, true));
                }
            }
        }

        return node;
    }

    private bool IsDescendantOf(FileSystemElement potentialDescendant, FileSystemElement? potentialAncestor)
    {
        if (potentialAncestor == null)
            return false;

        var current = potentialDescendant.Parent;
        while (current != null)
        {
            if (current == potentialAncestor)
                return true;
            current = current.Parent;
        }
        return false;
    }
}