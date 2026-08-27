using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using BouNodeKiller.Infrastructure;
using BouNodeKiller.Models;
using BouNodeKiller.Services;

namespace BouNodeKiller.ViewModels;

public sealed class MainWindowViewModel : ObservableObject
{
    private readonly NodeProcessScanner _scanner = new();
    private readonly NodeProcessKiller _killer = new();
    private readonly ObservableCollection<NodeProcessInfo> _processes = new();
    private readonly ICollectionView _processesView;
    private NodeProcessInfo? _selectedProcess;
    private string _searchText = string.Empty;
    private bool _isBusy;
    private string _statusMessage = "Prêt";
    private int _lastScanCount;

    public MainWindowViewModel()
    {
        _processesView = CollectionViewSource.GetDefaultView(_processes);
        _processesView.Filter = FilterProcess;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        KillSelectedCommand = new AsyncRelayCommand(KillSelectedAsync, CanKillSelected);
        KillAllCommand = new AsyncRelayCommand(KillAllAsync, CanKillAny);
    }

    public ICollectionView ProcessesView => _processesView;

    public ICommand RefreshCommand { get; }

    public ICommand KillSelectedCommand { get; }

    public ICommand KillAllCommand { get; }

    public NodeProcessInfo? SelectedProcess
    {
        get => _selectedProcess;
        set
        {
            if (SetProperty(ref _selectedProcess, value))
            {
                RaiseCommandStates();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _processesView.Refresh();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                RaiseCommandStates();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public int LastScanCount
    {
        get => _lastScanCount;
        private set => SetProperty(ref _lastScanCount, value);
    }

    public string AppVersion
        => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
           ?? "1.0.0";

    private bool CanKillSelected() => !IsBusy && SelectedProcess is not null;

    private bool CanKillAny() => !IsBusy && _processes.Count > 0;

    public async Task RefreshAsync()
    {
        IsBusy = true;
        StatusMessage = "Analyse des processus Node en cours...";

        try
        {
            var scannedProcesses = await Task.Run(() => _scanner.Scan());

            _processes.Clear();
            foreach (var process in scannedProcesses)
            {
                _processes.Add(process);
            }

            _processesView.Refresh();
            LastScanCount = _processes.Count;
            StatusMessage = LastScanCount == 0
                ? "Aucun processus Node détecté"
                : $"{LastScanCount} processus Node détecté(s)";
        }
        catch (Exception exception)
        {
            StatusMessage = $"Erreur pendant l'analyse: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task KillSelectedAsync()
    {
        var selectedProcess = SelectedProcess;
        if (selectedProcess is null)
        {
            StatusMessage = "Aucun processus sélectionné.";
            return;
        }

        var confirmation = System.Windows.MessageBox.Show(
            $"Fermer le processus PID {selectedProcess.ProcessId} ?",
            "Confirmer la fermeture",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (confirmation != System.Windows.MessageBoxResult.Yes)
        {
            return;
        }

        await Task.Run(() => _killer.Kill(new[] { selectedProcess }));
        await RefreshAsync();
    }

    private async Task KillAllAsync()
    {
        if (_processes.Count == 0)
        {
            StatusMessage = "Aucun processus Node à fermer.";
            return;
        }

        var confirmation = System.Windows.MessageBox.Show(
            $"Fermer tous les processus Node ({_processes.Count}) ?",
            "Confirmer la fermeture globale",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (confirmation != System.Windows.MessageBoxResult.Yes)
        {
            return;
        }

        var snapshot = _processes.ToArray();
        await Task.Run(() => _killer.Kill(snapshot));
        await RefreshAsync();
    }

    private bool FilterProcess(object value)
    {
        if (value is not NodeProcessInfo process)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        return Contains(process.ProcessId.ToString(), SearchText)
               || Contains(process.Name, SearchText)
               || Contains(process.ExecutionTarget, SearchText)
               || Contains(process.CommandLine, SearchText)
               || Contains(process.Owner, SearchText)
               || Contains(process.ExecutablePath, SearchText);
    }

    private static bool Contains(string? source, string value)
        => !string.IsNullOrWhiteSpace(source)
           && source.Contains(value, StringComparison.OrdinalIgnoreCase);

    private void RaiseCommandStates()
    {
        if (RefreshCommand is AsyncRelayCommand refreshCommand)
        {
            refreshCommand.RaiseCanExecuteChanged();
        }

        if (KillSelectedCommand is AsyncRelayCommand killSelectedCommand)
        {
            killSelectedCommand.RaiseCanExecuteChanged();
        }

        if (KillAllCommand is AsyncRelayCommand killAllCommand)
        {
            killAllCommand.RaiseCanExecuteChanged();
        }
    }
}
