using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FileSystem.UI.ViewModels;

public class MethodWrapper
{
    public MethodInfo Method { get; }
    public string Name => Method.Name;
    public ObservableCollection<ParameterViewModel> Parameters { get; }

    public MethodWrapper(MethodInfo method)
    {
        Method = method;
        Parameters = new ObservableCollection<ParameterViewModel>();
        foreach (var param in method.GetParameters())
            Parameters.Add(new ParameterViewModel(param));
    }
}

public partial class ParameterViewModel : ObservableObject
{
    public ParameterInfo Info { get; } = null!;
    public string Name => Info.Name ?? "unknown";
    public string Type => Info.ParameterType?.Name ?? "unknown";

    [ObservableProperty]
    private string _value = string.Empty;

    public ParameterViewModel(ParameterInfo info)
    {
        Info = info;
    }
}

public static class ReflectionHelpers
{
    public static object? ConvertParameter(string? input, Type targetType)
    {
        if (targetType == typeof(string))
            return input ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return targetType.IsValueType ? (object?)Activator.CreateInstance(targetType) : null;

        if (targetType.IsEnum)
            return Enum.Parse(targetType, input, ignoreCase: true);

        if (targetType.IsArray && targetType.GetElementType() == typeof(string))
        {
            return input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        try
        {
            return Convert.ChangeType(input, targetType);
        }
        catch
        {
            return targetType.IsValueType ? (object?)Activator.CreateInstance(targetType) : null;
        }
    }
}