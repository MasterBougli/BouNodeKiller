namespace BouNodeKiller.Models;

public sealed class NodeProcessInfo
{
    public int ProcessId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string ExecutablePath { get; init; } = string.Empty;

    public string CommandLine { get; init; } = string.Empty;

    public string ExecutionTarget { get; init; } = string.Empty;

    public string WorkingDirectory { get; init; } = string.Empty;

    public string Owner { get; init; } = string.Empty;

    public int ParentProcessId { get; init; }

    public DateTime? StartedAt { get; init; }

    public string StartedAtDisplay => StartedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";

    public string ShortCommandLine => CommandLine.Length <= 140 ? CommandLine : CommandLine[..137] + "...";

    public string ShortExecutablePath => ExecutablePath.Length <= 120 ? ExecutablePath : ExecutablePath[..117] + "...";

    public string ShortWorkingDirectory => WorkingDirectory.Length <= 120 ? WorkingDirectory : WorkingDirectory[..117] + "...";
}
