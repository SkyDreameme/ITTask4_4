using Avalonia.Data.Converters;
using FileSystem.Core.Models;
using System;
using System.Globalization;

namespace FileSystem.UI.ViewModels;

public class ElementTypeIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ElementType type)
        {
            return type == ElementType.Folder ? "📁" : "📄";
        }
        return "📄";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
