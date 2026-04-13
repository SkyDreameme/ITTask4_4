using System.Collections.ObjectModel;

namespace FileSystem.Core.Models;

public abstract class FileSystemElement
{
    public string Name { get; set; }

    public FolderElement? Parent { get; set; }

    public string Location
    {
        get
        {
            if (Parent == null)
                return Name;

            return $"{Parent.Location}/{Name}";
        }
    }

    public abstract ElementType Type { get; }

    public abstract long Size { get; }

    public abstract ObservableCollection<FileSystemElement> Children { get; }

    protected FileSystemElement(string name, FolderElement? parent)
    {
        Name = name;
        Parent = parent;
    }

    protected static string GenerateUniqueName(string baseName, FolderElement targetFolder)
    {
        string nameWithoutExt;
        string extension = "";

        var lastDot = baseName.LastIndexOf('.');
        if (lastDot > 0)
        {
            nameWithoutExt = baseName.Substring(0, lastDot);
            extension = baseName.Substring(lastDot);
        }
        else
        {
            nameWithoutExt = baseName;
        }

        string newName = baseName;
        int counter = 1;

        while (targetFolder.Children.Any(c => c.Name == newName))
        {
            newName = $"{nameWithoutExt}{counter}{extension}";
            counter++;
        }

        return newName;
    }

    public static FileSystemElement Copy(FileSystemElement source, FolderElement targetFolder)
    {
        if (source is FileElement file)
        {
            string uniqueName = GenerateUniqueName(file.Name, targetFolder);
            var newFile = new FileElement(uniqueName, targetFolder, file.Size);
            return newFile;
        }
        else if (source is FolderElement folder)
        {
            string uniqueName = GenerateUniqueName(folder.Name, targetFolder);
            var newFolder = new FolderElement(uniqueName, targetFolder);

            foreach (var child in folder.Children)
            {
                Copy(child, newFolder);
            }
            return newFolder;
        }

        throw new InvalidOperationException("Неизвестный тип элемента");
    }

    public static (bool Success, string Message) Move(FileSystemElement source, FolderElement targetFolder)
    {
        if (source == null)
            return (false, "❌ Ошибка: исходный элемент не найден");

        if (targetFolder == null)
            return (false, "❌ Ошибка: целевая папка не найдена");

        if (source.Parent == targetFolder)
            return (false, "ℹ️ Элемент уже находится в этой папке");

        if (IsDescendantOf(targetFolder, source))
            return (false, "❌ Нельзя перемещать папку в её собственную дочернюю папку");

        try
        {
            if (source.Parent != null)
            {
                source.Parent.Children.Remove(source);
            }

            source.Parent = targetFolder;

            if (!targetFolder.Children.Contains(source))
            {
                targetFolder.Children.Add(source);
            }

            return (true, $"✅ Элемент '{source.Name}' перемещён в {targetFolder.Name}");
        }
        catch (Exception ex)
        {
            return (false, $"❌ Ошибка при перемещении: {ex.Message}");
        }
    }

    private static bool IsDescendantOf(FileSystemElement? potentialDescendant, FileSystemElement? potentialAncestor)
    {
        if (potentialDescendant == null || potentialAncestor == null)
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