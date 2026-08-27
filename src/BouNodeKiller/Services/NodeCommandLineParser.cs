using System.Text;

namespace BouNodeKiller.Services;

public static class NodeCommandLineParser
{
    public static string GetExecutionTarget(string? commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return "Commande indisponible";
        }

        var tokens = Tokenize(commandLine);
        if (tokens.Count <= 1)
        {
            return "Node sans script";
        }

        for (var index = 1; index < tokens.Count; index++)
        {
            var token = tokens[index];

            if (IsInlineExecutionFlag(token))
            {
                return "[eval]";
            }

            if (IsFlag(token))
            {
                continue;
            }

            return token;
        }

        return "Arguments seuls";
    }

    private static bool IsInlineExecutionFlag(string token)
        => token.Equals("-e", StringComparison.OrdinalIgnoreCase)
           || token.Equals("--eval", StringComparison.OrdinalIgnoreCase)
           || token.Equals("-p", StringComparison.OrdinalIgnoreCase)
           || token.Equals("--print", StringComparison.OrdinalIgnoreCase);

    private static bool IsFlag(string token) => token.StartsWith("-", StringComparison.Ordinal);

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
}
