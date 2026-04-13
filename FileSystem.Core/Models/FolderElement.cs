using System.Collections.ObjectModel;
using System.Linq;

namespace FileSystem.Core.Models;

public class FolderElement : FileSystemElement
{
    private readonly ObservableCollection<FileSystemElement> _children;

    public FolderElement(string name, FolderElement? parent)
        : base(name, parent)
    {
        _children = new ObservableCollection<FileSystemElement>();
    }

    public override ElementType Type => ElementType.Folder;

    public override long Size => _children.Sum(child => child.Size);

    public override ObservableCollection<FileSystemElement> Children => _children;
}
