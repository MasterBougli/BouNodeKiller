using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace BouNodeKiller;

public partial class AboutWindow : Window
{
    public AboutWindow(string version)
    {
        InitializeComponent();
        DataContext = new AboutWindowViewModel(version);
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        e.Handled = true;
    }
}

internal sealed class AboutWindowViewModel
{
    public AboutWindowViewModel(string version)
    {
        VersionLabel = $"Version {version}";
        RepositoryUrl = new Uri("https://github.com/MasterBougli/BouNodeKiller");
        ReleasesUrl = new Uri("https://github.com/MasterBougli/BouNodeKiller/releases");
    }

    public string VersionLabel { get; }

    public Uri RepositoryUrl { get; }

    public Uri ReleasesUrl { get; }
}
