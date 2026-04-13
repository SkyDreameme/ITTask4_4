using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.Core.Models;

public class FileSystemAnalyzer : IFileSystemPlugin
{
    public FileSystemAnalyzer() { }

    public string FormatFileInfo(string fileName, long sizeInBytes)
    {
        return $"📄 Файл: {fileName} | Размер: {sizeInBytes:N0} Б";
    }

    public bool IsLargeFolder(long sizeThreshold)
    {
        return sizeThreshold > 1_048_576; // > 1 МБ
    }

    public string BuildPath(string volume, params string[] folderNames)
    {
        var path = $"{volume}:\\";
        return string.Join("\\", new[] { path }.Concat(folderNames)).TrimEnd('\\');
    }
}
