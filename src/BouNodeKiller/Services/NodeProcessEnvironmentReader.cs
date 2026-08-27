using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BouNodeKiller.Services;

internal static class NodeProcessEnvironmentReader
{
    private const int ProcessQueryInformation = 0x0400;
    private const int ProcessVmRead = 0x0010;
    private const int ProcessBasicInformationClass = 0;
    private const int PebProcessParametersOffsetX64 = 0x20;
    private const int CurrentDirectoryOffsetX64 = 0x38;

    public static string ResolveWorkingDirectory(int processId, string commandLine)
    {
        if (TryReadWorkingDirectory(processId, out var workingDirectory))
        {
            return workingDirectory;
        }

        if (TryInferFromCommandLine(commandLine, out var inferredDirectory))
        {
            return inferredDirectory;
        }

        return "Inconnu";
    }

    private static bool TryReadWorkingDirectory(int processId, out string workingDirectory)
    {
        workingDirectory = string.Empty;

        if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess)
        {
            var access = ProcessQueryInformation | ProcessVmRead;
            var processHandle = OpenProcess(access, false, processId);
            if (processHandle == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                var basicInformation = new ProcessBasicInformation();
                var status = NtQueryInformationProcess(
                    processHandle,
                    ProcessBasicInformationClass,
                    ref basicInformation,
                    Marshal.SizeOf<ProcessBasicInformation>(),
                    out _);

                if (status != 0 || basicInformation.PebBaseAddress == IntPtr.Zero)
                {
                    return false;
                }

                var processParametersPointer = ReadPointer(processHandle, basicInformation.PebBaseAddress + PebProcessParametersOffsetX64);
                if (processParametersPointer == IntPtr.Zero)
                {
                    return false;
                }

                var currentDirectoryString = ReadUnicodeString(processHandle, processParametersPointer + CurrentDirectoryOffsetX64);
                if (string.IsNullOrWhiteSpace(currentDirectoryString))
                {
                    return false;
                }

                workingDirectory = currentDirectoryString;
                return true;
            }
            finally
            {
                CloseHandle(processHandle);
            }
        }

        return false;
    }

    private static bool TryInferFromCommandLine(string commandLine, out string inferredDirectory)
    {
        inferredDirectory = string.Empty;

        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return false;
        }

        var tokens = Tokenize(commandLine);
        foreach (var token in tokens.Skip(1))
        {
            if (token.StartsWith("-", StringComparison.Ordinal))
            {
                continue;
            }

            var candidate = token.Trim('"');
            if (Path.IsPathRooted(candidate))
            {
                var directory = Path.GetDirectoryName(candidate);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    inferredDirectory = directory;
                    return true;
                }
            }

            break;
        }

        return false;
    }

    private static string ReadUnicodeString(IntPtr processHandle, IntPtr address)
    {
        var unicodeString = ReadStruct<UnicodeString>(processHandle, address);
        if (unicodeString.Buffer == IntPtr.Zero || unicodeString.Length == 0)
        {
            return string.Empty;
        }

        var byteCount = unicodeString.Length;
        var buffer = new byte[byteCount];

        if (!ReadProcessMemory(processHandle, unicodeString.Buffer, buffer, buffer.Length, out var bytesRead))
        {
            return string.Empty;
        }

        if (bytesRead == 0)
        {
            return string.Empty;
        }

        return Encoding.Unicode.GetString(buffer, 0, (int)bytesRead).TrimEnd('\0');
    }

    private static IntPtr ReadPointer(IntPtr processHandle, IntPtr address)
    {
        var pointerSize = IntPtr.Size;
        var buffer = new byte[pointerSize];

        if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out var bytesRead) || bytesRead != pointerSize)
        {
            return IntPtr.Zero;
        }

        return pointerSize == 8
            ? new IntPtr(BitConverter.ToInt64(buffer, 0))
            : new IntPtr(BitConverter.ToInt32(buffer, 0));
    }

    private static T ReadStruct<T>(IntPtr processHandle, IntPtr address) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var buffer = new byte[size];

        if (!ReadProcessMemory(processHandle, address, buffer, buffer.Length, out var bytesRead) || bytesRead != size)
        {
            return default;
        }

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }

    private static List<string> Tokenize(string commandLine)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var character in commandLine)
        {
            if (character == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(character) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Clear();
                }

                continue;
            }

            current.Append(character);
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out nint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessBasicInformation
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UnicodeString
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }
}
