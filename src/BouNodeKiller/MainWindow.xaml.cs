using System.Windows;
using BouNodeKiller.ViewModels;

namespace BouNodeKiller;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.RefreshAsync();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow(_viewModel.AppVersion)
        {
            Owner = this
        };

        aboutWindow.ShowDialog();
    }
}
