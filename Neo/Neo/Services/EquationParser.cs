using System.Text;

namespace Neo.Services;

public sealed class EquationParser : Parser
{
    public EquationParser(string input) : base(input.GetDigits())
    {
    }
}

public static partial class ListExtension
{
    public static string GetDigits(this string input)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            // if input[i] neither is digit nor is split symbol ";"
            // cycle is iterating
            if (!char.IsDigit(input[i]) && input[i] != Parser.SplitSymbol)
                continue;

            // else adds to string builder input[i] 
            if (char.IsDigit(input[i]) || input[i] == Parser.SplitSymbol)
                sb.Append(input[i]);

            // stop cycle if "i" more than length of input string
            if (i >= input.Length - 1)
                break;

            // if next element of input neither is digit nor is split symbol ";"
            // adds whitespace
            if (!char.IsDigit(input[i + 1]) && input[i + 1] != Parser.SplitSymbol)
                sb.Append(' ');
        }

        return sb.ToString();
    }
}