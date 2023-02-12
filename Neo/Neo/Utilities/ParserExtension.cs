using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using __livexaml;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using Xamarin.Forms.Internals;

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
    public static string GetUnknownVariables(this string input, bool fullInput = false)
    {
        if (!fullInput)
            return new string(input.Where(char.IsLetter).Distinct().ToArray());

        var digits = new string(input.Separate().OrderByDescending(x => x.Length).ToList()[0].ToArray());
        return new string(digits.Where(char.IsLetter).Distinct().ToArray());
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
            var index = input[i];
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
                                            && input[i + 1] != Parser.FloatSymbolComma
                                            && input[i + 1] != ' ')
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
    /// counts symbols in <see cref="input"/>
    /// </summary>
    /// <param name="input">read text</param>
    /// <param name="symbol">symbol for counting</param>
    /// <returns></returns>
    public static int SymbolCount(this string input, char symbol)
    {
        return input.Count(x => x == symbol);
    }

    public static string OnZeroVariable(this string input)
    {
        var equations = input.Separate().AppendZeroCoefficients(input.GetUnknownVariables());
        return "";
    }

    /// <summary>
    /// appends in internal <see cref="List{T}"/> zero coefficients of equations
    /// </summary>
    /// <param name="equations"></param>
    /// <param name="unknownVariablesDict"></param>
    /// <returns></returns>
    private static List<string> AppendZeroCoefficients(this List<string> equations, string unknownVariables)
    {
        equations[0] = " 2y = 4";
        var appendableVariables = equations.GetAppendableEquations(unknownVariables);
        var digitEquations = appendableVariables.Combine().GetDigits().Separate();

        var output = new List<string>();


        var sb = new StringBuilder();

        for (var i = 0; i < digitEquations.Count; i++)
        {
            var missedVariables = equations.GetVariableNames(unknownVariables, i);
            for (var j = 0; j < missedVariables.Count; j++)
            {
                if (missedVariables[j].Values.All(x => x == i))
                {
                    sb.Append($" {digitEquations[i]} ");
                    break;
                }

                sb.Append(" 0 ");
                break;
            }
        }


        return output;
    }

    private static string RemoveWhiteSpace(this string str)
    {
        return new string(str.Where(x => !char.IsWhiteSpace(x)).ToArray());
    }

    private static string Trim(this IEnumerable<string> list)
    {
        return list.Select(item => item.Trim()).ToList().Combine();
    }

    private static string GetMissedUnknownVariables(this string equation, string unknownVariables)
    {
        var sb = new StringBuilder();
        foreach (var unknownVariable in unknownVariables
                     .Where(unknownVariable => !equation.Contains(unknownVariable.ToString())))
            sb.Append(unknownVariable);

        return sb.ToString();
    }

    private static Dictionary<char, int> GetIndices(this string unknownVariables)
    {
        var indices = new Dictionary<char, int>();
        for (var i = 0; i < unknownVariables.Length; i++)
            indices.Add(unknownVariables[i], i);
        return indices;
    }

    private static List<Dictionary<char, int>> GetVariableNames(this List<string> equations, string unknownVariables,
        int index)
    {
        var needToAppend = new List<Dictionary<char, int>>();
        var indices = unknownVariables.GetIndices();

        for (var i = 0; i < unknownVariables.Length; i++)
        {
            if (!equations[index].Contains(unknownVariables[i]))
                needToAppend.Add(new Dictionary<char, int>
                {
                    { unknownVariables[i], indices.Values.FirstOrDefault(x => x == i) }
                });
        }

        return needToAppend;
    }

    private static List<Dictionary<char, int>> GetVariableNames(this List<string> equations, string unknownVariables)
    {
        var needToAppend = new List<Dictionary<char, int>>();

        foreach (var equation in equations)
        {
            foreach (var unknownVariable in unknownVariables)
            {
                if (!equation.Contains(unknownVariable))
                    needToAppend.Add(new Dictionary<char, int>
                    {
                        { unknownVariable, equations.IndexOf(equation) }
                    });
            }
        }

        return needToAppend;
    }

    private static List<string> GetAppendableEquations(this List<string> equations, string unknownVariables)
    {
        var needToAppend = new List<string>();

        foreach (var equation in equations)
        {
            foreach (var unknownVariable in unknownVariables)
            {
                if (!equation.Contains(unknownVariable))
                    needToAppend.Add(equation);
            }
        }

        return needToAppend.Distinct().ToList();
    }

    /// <summary>
    /// combines <see cref="List{T}"/> in one string with <see cref="splitSymbol"/>
    /// </summary>
    /// <param name="list">list of <see cref="T"/></param>
    /// <param name="splitSymbol">symbol for splitting</param>
    /// <returns><see cref="string"/> with <see cref="splitSymbol"/> in indices where list was ended</returns>
    private static string Combine<T>(this List<T> list, char splitSymbol = Parser.SplitSymbol)
    {
        var sb = new StringBuilder();
        foreach (var equation in list)
            sb.Append($"{equation}{splitSymbol}");
        return sb.ToString();
    }

    /// <summary>
    /// separates <see cref="input"/> in <see cref="List{T}"/> using <see cref="splitSymbol"/>
    /// </summary>
    /// <param name="input">string for separating</param>
    /// <param name="splitSymbol">symbol for splitting</param>
    /// <returns><see cref="List{T}"/> from separated <see cref="input"/></returns>
    private static List<string> Separate(this string input, char splitSymbol = Parser.SplitSymbol)
    {
        var list = new List<string>();
        var sb = new StringBuilder();
        foreach (var item in input)
        {
            if (item != splitSymbol)
            {
                sb.Append(item);
            }
            else
            {
                list.Add(sb.ToString());
                sb.Clear();
            }
        }

        return list;
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
        var digits = new List<double>();

        for (var i = 0; i < input.Length; i++)
        {
            if (input[i] == ' ')
                continue;
            sb.Append(input[i]);

            if (input[i + 1] != ' ')
                continue;

            digits.Add(double.Parse(sb.ToString()));
            sb.Clear();
        }

        return digits;
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

            if (input[i] != input.GetUnknownVariables()[j] || char.IsDigit(input[i - 1]))
                continue;
            sb.Append("1 ");
            return true;
        }

        return false;
    }
}