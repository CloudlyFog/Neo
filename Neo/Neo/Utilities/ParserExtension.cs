using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace Neo.Utilities;

/// <summary>
/// describe extension class for <see cref="Parser"/>
/// </summary>
public static class ParserExtension
{
    /// <summary>
    /// returns string with unknown variables from system linear equations
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <returns></returns>
    public static string GetUnknownVariables(this string input)
    {
        return new string(input.Where(char.IsLetter).Distinct().ToArray());
    }

    /// <summary>
    /// removes elements every time when i in cycle will divide without a trace by every' value
    /// after removes white spaces and empty strings from list
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="every">position to delete</param>
    /// <param name="rows">count of rows from string of parsed matrix</param>
    /// <returns></returns>
    public static List<string> RemoveEvery(this List<string> input, int every, int rows)
    {
        if (Parser.ValidArray(input.ToArray(), nameof(input)) is null)
            return null;
        for (var i = 1; i <= rows; i++)
            input.RemoveAt(every * i);

        return input.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList();
    }

    /// <summary>
    /// returns new <see cref="List{T}"/> with elements which were at point equals every' value
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="every">position to get</param>
    /// <param name="rows">count of rows from string of parsed matrix</param>
    /// <returns></returns>
    public static List<string> AddEvery(this List<string> input, int every, int rows)
    {
        if (Parser.ValidArray(input.ToArray(), nameof(input)) is null)
            return null;
        var output = Enumerable.Empty<string>();
        for (var i = 1; i <= rows; i++)
            output = output.Append(input[every * i - 1]);

        return output.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList();
    }

    /// <summary>
    /// gets as arg string with/without other symbols like words, punctuations marks etc
    /// return new <see cref="string"/> only with digits
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <returns></returns>
    public static string GetDigits(this string input)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            if (OnNegativeSymbol(input, sb, i) || OnFloatSymbol(input, sb, i))
                continue;

            if (OnUnitVariable(input, sb, i))
                continue;

            // if input[i] neither is digit nor is split symbol ";" cycle 'll iterate
            if (!char.IsDigit(input[i]) && input[i] != Parser.SplitSymbol)
                continue;

            sb.Append(input[i]);

            // stop cycle if "i" more than length of input string
            if (i >= input.Length - 1)
                break;

            // if the next input element is neither ";", nor ",", nor ".", nor a digit
            // adds whitespace
            if (!char.IsDigit(input[i + 1]) && input[i + 1] != Parser.SplitSymbol
                                            && input[i + 1] != Parser.FloatSymbolDot
                                            && input[i + 1] != Parser.FloatSymbolComma)
            {
                sb.Append(' ');
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// check passed string of input it's trash input or not
    /// </summary>
    /// <param name="input">read text</param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsTrash(this string input)
    {
        if (!input.Any(item => "1234567890".Any(numeric => item == numeric)))
            return true;
        var len = input.Count(x => x == Parser.SplitSymbol);
        if (len is <= 0 or > 5)
            return true;
        return input.GetUnknownVariables().Length != input.ConvertToDigits().Count() / len - 1;
    }

    /// <summary>
    /// cleans <see cref="input"/> from whitespaces before <see cref="splitSymbol"/>
    /// in order to verify validation of <see cref="input"/> in the next operations of solving
    /// </summary>
    /// <param name="input">text</param>
    /// <param name="splitSymbol">symbol for splitting equations</param>
    /// <returns></returns>
    public static string RemoveWhiteSpacesBeforeSeparator(this string input, char splitSymbol = Parser.SplitSymbol)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            if (i >= input.Length - 1)
                return $"{sb}{splitSymbol}";
            if (input[i + 1] == splitSymbol)
                continue;
            sb.Append(input[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// returns sequence of float digits which was parsed from <see cref="input"/>
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <returns></returns>
    private static IEnumerable<double> ConvertToDigits(this string input)
    {
        input = input.GetDigits()
            .Replace(Parser.SplitSymbol.ToString(), " ")
            .Replace(Parser.NegativeSymbol.ToString(), "");

        var sb = new StringBuilder();
        var array = new List<double>();

        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == ' ')
                continue;
            sb.Append(input[i]);

            if (input[i + 1] != ' ')
                continue;

            array.Add(double.Parse(sb.ToString()));
            sb.Clear();
        }

        return array;
    }

    /// <summary>
    /// adds to <see cref="StringBuilder"/> negative symbols if they are
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="sb">used <see cref="StringBuilder"/></param>
    /// <param name="i">current index</param>
    /// <returns></returns>
    private static bool OnNegativeSymbol(string input, StringBuilder sb, int i)
    {
        if (input[i] != Parser.NegativeSymbol)
            return false;

        for (var j = i; j < input.Length; j++)
        {
            if (!char.IsDigit(input[j]))
                continue;
            sb.Append(Parser.NegativeSymbol);
            break;
        }

        return true;
    }

    /// <summary>
    /// adds to <see cref="StringBuilder"/> float symbols if they are
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="sb">used <see cref="StringBuilder"/></param>
    /// <param name="i">current index</param>
    /// <returns></returns>
    private static bool OnFloatSymbol(string input, StringBuilder sb, int i)
    {
        if (input[i] != Parser.FloatSymbolDot && input[i] != Parser.FloatSymbolComma)
            return false;

        sb.Append(Parser.FloatSymbolDot);

        return true;
    }

    /// <summary>
    /// adds to <see cref="StringBuilder"/> "1 " if there isn't coefficient of the nearest unknown variable 
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="sb">used <see cref="StringBuilder"/></param>
    /// <param name="i">current index</param>
    /// <returns></returns>
    private static bool OnUnitVariable(string input, StringBuilder sb, int i)
    {
        for (var j = 0; j < input.GetUnknownVariables().Length; j++)
        {
            if (input[i] == input.GetUnknownVariables()[j] && i == 0)
            {
                sb.Append("1 ");
                return true;
            }

            //if (input[i] != Solver.Input.GetUnknownVariables()[j] || char.IsDigit(input[i - 1]))
            if (input[i] != Solver.Input.GetUnknownVariables()[j] || char.IsDigit(input[i - 1]))
                continue;
            sb.Append("1 ");
            return true;
        }

        return false;
    }
}