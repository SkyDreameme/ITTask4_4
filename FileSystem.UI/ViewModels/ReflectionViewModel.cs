using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileSystem.UI.ViewModels;

public partial class ReflectionViewModel : ViewModelBase
{
    [ObservableProperty] private string _libraryPath = string.Empty;
    [ObservableProperty] private string _statusMessage = "Укажите путь к .dll и нажмите 'Загрузить'";
    [ObservableProperty] private ObservableCollection<string> _availableClasses = new();
    [ObservableProperty] private string? _selectedClass;
    [ObservableProperty] private ObservableCollection<MethodWrapper> _methods = new();
    [ObservableProperty] private MethodWrapper? _selectedMethod;
    [ObservableProperty] private string _executionResult = string.Empty;

    private Assembly? _loadedAssembly;
    private readonly Dictionary<string, Type> _typeCache = new();

    [RelayCommand]
    private void LoadAssembly()
    {
        if (string.IsNullOrWhiteSpace(LibraryPath) || !File.Exists(LibraryPath))
        {
            StatusMessage = "❌ Неверный путь к файлу.";
            return;
        }

        try
        {
            _loadedAssembly = Assembly.LoadFrom(LibraryPath);
            DiscoverPluginTypes();
            StatusMessage = AvailableClasses.Count > 0
                ? $"✅ Найдено плагинов: {AvailableClasses.Count}"
                : "⚠️ Классы, реализующие IFileSystemPlugin, не найдены.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Ошибка загрузки сборки: {ex.Message}";
        }
    }

    private void DiscoverPluginTypes()
    {
        AvailableClasses.Clear();
        _typeCache.Clear();
        if (_loadedAssembly == null) return;

        var pluginTypes = _loadedAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces().Any(i => i.Name == "IFileSystemPlugin"));

        foreach (var type in pluginTypes)
        {
            AvailableClasses.Add(type.Name);
            _typeCache[type.Name] = type;
        }
    }

    partial void OnSelectedClassChanged(string? value)
    {
        Methods.Clear();
        SelectedMethod = null;
        ExecutionResult = string.Empty;

        if (string.IsNullOrEmpty(value) || !_typeCache.TryGetValue(value, out var type)) return;

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.DeclaringType != typeof(object));

        foreach (var method in methods)
            Methods.Add(new MethodWrapper(method));

        ExecuteMethodCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedMethodChanged(MethodWrapper? value)
    {
        ExecuteMethodCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteMethod))]
    private void ExecuteMethod()
    {
        if (SelectedMethod == null || !_typeCache.TryGetValue(SelectedClass!, out var type)) return;

        try
        {
            var instance = Activator.CreateInstance(type);
            var parameters = SelectedMethod.Parameters.Select(p =>
                ReflectionHelpers.ConvertParameter(p.Value, p.Info.ParameterType)).ToArray();

            var result = SelectedMethod.Method.Invoke(instance, parameters);
            ExecutionResult = $"✅ Результат: {result?.ToString() ?? "void"}";
        }
        catch (Exception ex)
        {
            ExecutionResult = $"❌ Ошибка выполнения: {ex.InnerException?.Message ?? ex.Message}";
        }
    }

    private bool CanExecuteMethod() => SelectedMethod != null && !string.IsNullOrEmpty(SelectedClass);
}