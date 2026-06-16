using Domain;

namespace DPAT_1_Femke_Bas.Parser;

// Parses the FSM text format and feeds a StateMachineBuilder.
//
// Format tolerance: the parser accepts TRANSITION lines both with and without the optional "->"
// separator between source and destination, and with or without a quoted guard string.
// The actual fixture files (e.g. FSM1.txt) omit "->" and trailing guard quotes for transitions
// without a guard — both variants parse correctly.
public class FsmFileParser
{
    private record struct Token(string Value, bool IsQuoted);

    public StateMachine Parse(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"FSM file not found: {filePath}");

        using var reader = new StreamReader(filePath);
        return Parse(reader, Path.GetFileNameWithoutExtension(filePath));
    }

    public StateMachine Parse(TextReader reader, string name)
    {
        var builder = new StateMachineBuilder().WithName(name);

        var lines = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
            lines.Add(line);

        for (var lineNumber = 1; lineNumber <= lines.Count; lineNumber++)
        {
            var raw = lines[lineNumber - 1];
            var stripped = StripComment(raw).Trim();
            if (stripped.EndsWith(';'))
                stripped = stripped[..^1].Trim();
            if (string.IsNullOrWhiteSpace(stripped))
                continue;

            var tokens = Tokenize(stripped);
            if (tokens.Count == 0) continue;

            var keyword = tokens[0].Value.ToUpperInvariant();
            try
            {
                switch (keyword)
                {
                    case "STATE":      ParseState(tokens, lineNumber, builder);      break;
                    case "TRIGGER":    ParseTrigger(tokens, lineNumber, builder);    break;
                    case "ACTION":     ParseAction(tokens, lineNumber, builder);     break;
                    case "TRANSITION": ParseTransition(tokens, lineNumber, builder); break;
                    default:
                        throw new FormatException($"Unknown keyword '{tokens[0].Value}'.");
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Line {lineNumber}: {ex.Message}", ex);
            }
        }

        return builder.Build();
    }

    // STATE <identifier> <parent> "<name>" : <state_type> ;
    private static void ParseState(List<Token> tokens, int lineNumber, StateMachineBuilder builder)
    {
        if (tokens.Count < 6)
            throw new FormatException(
                $"Line {lineNumber}: STATE requires 6 tokens (STATE id parent \"name\" : TYPE), got {tokens.Count}.");

        var identifier = tokens[1].Value;
        var parent = tokens[2].Value;
        var name = tokens[3].Value;
        // tokens[4] is ":"
        var stateType = ParseStateType(tokens[5].Value, lineNumber);
        string? parentId = parent == "_" ? null : parent;

        builder.AddState(identifier, parentId, name, stateType);
    }

    // TRIGGER <identifier> "<description>" ;
    private static void ParseTrigger(List<Token> tokens, int lineNumber, StateMachineBuilder builder)
    {
        if (tokens.Count < 3)
            throw new FormatException(
                $"Line {lineNumber}: TRIGGER requires 3 tokens (TRIGGER id \"description\"), got {tokens.Count}.");

        builder.AddTrigger(tokens[1].Value, tokens[2].Value);
    }

    // ACTION <ownerIdentifier> "<description>" : <action_type> ;
    private static void ParseAction(List<Token> tokens, int lineNumber, StateMachineBuilder builder)
    {
        if (tokens.Count < 5)
            throw new FormatException(
                $"Line {lineNumber}: ACTION requires 5 tokens (ACTION id \"description\" : TYPE), got {tokens.Count}.");

        var ownerIdentifier = tokens[1].Value;
        var description = tokens[2].Value;
        // tokens[3] is ":"
        var actionType = ParseActionType(tokens[4].Value, lineNumber);

        builder.AddAction(ownerIdentifier, description, actionType);
    }

    // TRANSITION <id> <source> [->] <destination> [<trigger>] ["<guard>"] ;
    private static void ParseTransition(List<Token> tokens, int lineNumber, StateMachineBuilder builder)
    {
        if (tokens.Count < 4)
            throw new FormatException(
                $"Line {lineNumber}: TRANSITION requires at least 4 tokens (TRANSITION id source dest), got {tokens.Count}.");

        var idx = 1;
        var id = tokens[idx++].Value;
        var sourceId = tokens[idx++].Value;

        // Optional "->" separator
        if (idx < tokens.Count && tokens[idx].Value == "->")
            idx++;

        if (idx >= tokens.Count)
            throw new FormatException($"Line {lineNumber}: TRANSITION '{id}' is missing destination state.");

        var destId = tokens[idx++].Value;

        string? triggerId = null;
        string? guard = null;

        while (idx < tokens.Count)
        {
            var token = tokens[idx++];
            if (token.IsQuoted)
                guard = string.IsNullOrEmpty(token.Value) ? null : token.Value;
            else
                triggerId = token.Value;
        }

        builder.AddTransition(id, sourceId, destId, triggerId, guard);
    }

    private static StateType ParseStateType(string value, int lineNumber) =>
        value.ToUpperInvariant() switch
        {
            "INITIAL"  => StateType.Initial,
            "SIMPLE"   => StateType.Simple,
            "COMPOUND" => StateType.Compound,
            "FINAL"    => StateType.Final,
            _ => throw new FormatException($"Line {lineNumber}: Unknown state type '{value}'.")
        };

    private static ActionType ParseActionType(string value, int lineNumber) =>
        value.ToUpperInvariant() switch
        {
            "ENTRY_ACTION"      => ActionType.EntryAction,
            "DO_ACTION"         => ActionType.DoAction,
            "EXIT_ACTION"       => ActionType.ExitAction,
            "TRANSITION_ACTION" => ActionType.TransitionAction,
            _ => throw new FormatException($"Line {lineNumber}: Unknown action type '{value}'.")
        };

    private static string StripComment(string line)
    {
        var inQuote = false;
        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '"') inQuote = !inQuote;
            else if (line[i] == '#' && !inQuote) return line[..i];
        }
        return line;
    }

    // Splits a line into tokens, treating quoted strings as single tokens (quotes are stripped).
    private static List<Token> Tokenize(string line)
    {
        var tokens = new List<Token>();
        int i = 0;
        while (i < line.Length)
        {
            while (i < line.Length && char.IsWhiteSpace(line[i])) i++;
            if (i >= line.Length) break;

            if (line[i] == '"')
            {
                i++; // skip opening quote
                var start = i;
                while (i < line.Length && line[i] != '"') i++;
                tokens.Add(new Token(line[start..i], IsQuoted: true));
                if (i < line.Length) i++; // skip closing quote
            }
            else
            {
                var start = i;
                while (i < line.Length && !char.IsWhiteSpace(line[i])) i++;
                tokens.Add(new Token(line[start..i], IsQuoted: false));
            }
        }
        return tokens;
    }
}
