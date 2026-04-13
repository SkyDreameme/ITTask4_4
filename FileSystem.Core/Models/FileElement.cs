using System.Collections.ObjectModel;

namespace FileSystem.Core.Models;

public class FileElement : FileSystemElement
{
    private readonly long _size;
    private readonly ObservableCollection<FileSystemElement> _emptyChildren;

    public FileElement(string name, FolderElement? parent, long size)
        : base(name, parent)
    {
        _size = size;
        _emptyChildren = new ObservableCollection<FileSystemElement>();
    }

    public override ElementType Type => ElementType.File;

    public override long Size => _size;

    public override ObservableCollection<FileSystemElement> Children => _emptyChildren;
}
