using System.Diagnostics;
using BouNodeKiller.Models;

namespace BouNodeKiller.Services;

public sealed class NodeProcessKiller
{
    public int Kill(IEnumerable<NodeProcessInfo> processes)
    {
        var killedCount = 0;

        foreach (var processInfo in processes.DistinctBy(item => item.ProcessId))
        {
            try
            {
                using var process = Process.GetProcessById(processInfo.ProcessId);
                process.Kill(entireProcessTree: true);
                killedCount++;
            }
            catch
            {
                // Ignore processes that already exited or are not accessible.
            }
        }

        return killedCount;
    }
}
