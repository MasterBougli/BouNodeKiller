using System.Management;
using BouNodeKiller.Models;

namespace BouNodeKiller.Services;

public sealed class NodeProcessScanner
{
    public IReadOnlyList<NodeProcessInfo> Scan()
    {
        var results = new List<NodeProcessInfo>();

        using var searcher = new ManagementObjectSearcher(
            "SELECT ProcessId, Name, CommandLine, ExecutablePath, ParentProcessId, CreationDate " +
            "FROM Win32_Process WHERE Name = 'node.exe' OR Name = 'nodejs.exe'");

        foreach (ManagementObject process in searcher.Get())
        {
            try
            {
                results.Add(CreateProcessInfo(process));
            }
            catch
            {
                // If a process disappears while we read it, ignore it and continue.
            }
        }

        return results.OrderBy(item => item.ProcessId).ToArray();
    }

    private static NodeProcessInfo CreateProcessInfo(ManagementObject process)
    {
        var processId = Convert.ToInt32(process["ProcessId"]);
        var name = Convert.ToString(process["Name"]) ?? "node.exe";
        var commandLine = Convert.ToString(process["CommandLine"]) ?? string.Empty;
        var executablePath = Convert.ToString(process["ExecutablePath"]) ?? string.Empty;
        var parentProcessId = Convert.ToInt32(process["ParentProcessId"]);
        var startedAt = ParseCreationDate(process["CreationDate"]);
        var owner = GetOwner(process);
        var workingDirectory = NodeProcessEnvironmentReader.ResolveWorkingDirectory(processId, commandLine);

        return new NodeProcessInfo
        {
            ProcessId = processId,
            Name = name,
            ExecutablePath = executablePath,
            CommandLine = commandLine,
            ExecutionTarget = NodeCommandLineParser.GetExecutionTarget(commandLine),
            WorkingDirectory = workingDirectory,
            Owner = owner,
            ParentProcessId = parentProcessId,
            StartedAt = startedAt
        };
    }

    private static DateTime? ParseCreationDate(object? rawValue)
    {
        if (rawValue is not string value || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return ManagementDateTimeConverter.ToDateTime(value);
        }
        catch
        {
            return null;
        }
    }

    private static string GetOwner(ManagementObject process)
    {
        try
        {
            var arguments = new object?[] { null, null };
            var returnValue = Convert.ToUInt32(process.InvokeMethod("GetOwner", arguments));

            if (returnValue != 0)
            {
                return "Inconnu";
            }

            var user = Convert.ToString(arguments[0]);
            var domain = Convert.ToString(arguments[1]);

            if (string.IsNullOrWhiteSpace(user))
            {
                return "Inconnu";
            }

            return string.IsNullOrWhiteSpace(domain) ? user : $"{domain}\\{user}";
        }
        catch
        {
            return "Inconnu";
        }
    }
}
