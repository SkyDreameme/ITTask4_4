using Avalonia.Controls;
using Avalonia.Interactivity;
using FileSystem.UI.ViewModels;

namespace FileSystem.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private async void OnMoveButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.MoveSelectedAsync(this);
        }
    }
}