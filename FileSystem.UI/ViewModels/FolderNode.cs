using FileSystem.Core.Models;
using System.Collections.ObjectModel;

namespace FileSystem.UI.ViewModels;

public class FolderNode
{
    public FolderElement Folder { get; }
    public bool CanExpand { get; }

    public ObservableCollection<FolderNode> Children { get; }

    public FolderNode(FolderElement folder, bool canExpand, ObservableCollection<FolderNode> children)
    {
        Folder = folder;
        CanExpand = canExpand;
        Children = children;
    }

    public string Name => Folder.Name;
    public string Location => Folder.Location;
}
