using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Exceptions;

namespace Neo.Services;

public class Parser
{
    public const char SplitSymbol = ';';

    /// <summary>
    /// conversed string of equation
    /// </summary>
    private readonly string _input;

    /// <summary>
    /// every what iteration 'll doing something
    /// </summary>
    private int _every = 1;

    public Parser(string input)
    {
        _input = input.GetDigits();
    }

    /// <summary>
    /// Take data from <see cref="_input"/> and put it to <see cref="Matrix{T}"/>
    /// Uses <see cref="_input"/> like 
    /// </summary>
    /// <returns></returns>
    public Matrix<double> MatrixConversion()
    {
        var targetArray = new double[
            // read count of ";" and therefore count will one less than actually
            _input.Count(x => x == SplitSymbol),
            // read count of spaces and divide it on count of symbol ";"
            _input.Count(x => x == ' ') / _input.Count(x => x == SplitSymbol)];

        _every = targetArray.GetLength(1);

        // removing white space and commas
        var filterResult = _input.Split(' ', SplitSymbol).Where(x => x is not (" " and "")).ToList()
            // get matrix
            .RemoveEvery(_every, targetArray.GetLength(0));

        return GetMatrixValue(targetArray, filterResult);
    }

    /// <summary>
    /// Take data from <see cref="_input"/> and put it to <see cref="Vector{T}"/>
    /// </summary>
    /// <returns></returns>
    public Vector<double> VectorConversion()
    {
        // remove white space and commas
        var filterResult = _input.Split(' ', SplitSymbol).Where(x => x is not (" " and "")).ToList();

        // in first part we find length of matrix and consequently found last index of matrix
        // as we know if we add 1 to the value of the last index of matrix we get length of all equation transformed to matrix
        // and therefore we get index in array which 'll specify to the right side of matrix equation
        // something like
        // 1 2 3 = 4
        // 5 6 7 = 8
        // 9 10 11 = 12
        // we get index of "3" and next add 1 for take index of "4"
        _every = _input.Count(x => x == ' ') / _input.Count(x => x == SplitSymbol) + 1;

        // remove empty space
        filterResult = filterResult.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList()
            // get vector
            .AddEvery(_every, _input.Count(x => x == SplitSymbol));

        return GetVectorValue(new double[filterResult.Count], filterResult);
    }

    /// <summary>
    /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
    /// </summary>
    /// <param name="targetArray"> describe only size of array</param>
    /// <param name="filterResult">data from ocr output</param>
    private static Matrix<double> GetMatrixValue(double[,] targetArray, List<string> filterResult)
    {
        if (targetArray is null)
            throw new ArgumentNullException(nameof(targetArray));

        if (targetArray.Length <= 0)
            throw new ArgumentException($"length of {nameof(targetArray)} less or equals 0");

        ValidArray(filterResult.ToArray(), nameof(filterResult));
        // start point for input text
        // like an iterator
        var point = 0;

        for (var i = 0; i < targetArray.GetLength(0); i++)
        {
            // on every iteration we increasing point instead of j
            // because we have to appeal to filterResult index instead of targetArray's index
            for (var j = 0; j < targetArray.GetLength(1); point++, j++)
            {
                if (!ValidIteration(ref point, filterResult))
                    break;
                try
                {
                    targetArray[i, j] = double.Parse(filterResult[point]);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        return Matrix<double>.Build.DenseOfArray(targetArray);
    }

    /// <summary>
    /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
    /// </summary>
    /// <param name="targetArray">array which will contain parsed data from <see cref="filterResult"/></param>
    /// <param name="filterResult">parsed filtered data from ocr</param>
    private static Vector<double> GetVectorValue(double[] targetArray, List<string> filterResult)
    {
        ValidArray(targetArray, nameof(targetArray));
        ValidArray(filterResult.ToArray(), nameof(filterResult));

        // start point for input text as iterator
        var point = 0;
        try
        {
            filterResult = filterResult
                .TakeWhile(item => ValidIteration(ref point, filterResult)).ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }


        for (var i = 0; i < filterResult.Count; i++)
        {
            try
            {
                targetArray[i] = double.Parse(filterResult[i]);
            }
            catch (ParserException exception)
            {
                Console.WriteLine(exception);
                throw new ParserException(exception.Message, filterResult[i]);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new InvalidOperationException(exception.Message, exception.InnerException);
            }
        }

        return Vector<double>.Build.DenseOfArray(targetArray);
    }

    /// <summary>
    /// Validate index of filterResult is corresponding to requirements or not
    /// </summary>
    /// <param name="point">index of <see cref="filterResult"/></param>
    /// <param name="filterResult">parsed data from Tesseract OCR</param>
    /// <returns></returns>
    private static bool ValidIteration(ref int point, List<string> filterResult)
    {
        if (point == filterResult.Count)
            return false;
        if (filterResult[point] != string.Empty)
            return true;
        point++;
        return false;
    }

    internal static void ValidArray<T>(T[] array, string arrayName)
    {
        if (array is null)
            throw new ArgumentNullException(arrayName);

        if (array.Length <= 0)
            throw new ArgumentException($"length of {arrayName} less or equals 0");
    }
}

public static partial class ListExtension
{
    /// <summary>
    /// remove elements every time when i in cycle will divide without a trace by every' value
    /// after removes white spaces and empty strings from list
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="every">position to delete</param>
    /// <param name="rows">count of rows from string of parsed matrix</param>
    /// <returns></returns>
    public static List<string> RemoveEvery(this List<string> input, int every, int rows)
    {
        Parser.ValidArray(input.ToArray(), nameof(input));
        for (var i = 1; i <= rows; i++)
            input.RemoveAt(every * i);

        return input.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList();
    }

    /// <summary>
    /// return new <see cref="List{T}"/> with elements which were at point equals every' value
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <param name="every">position to get</param>
    /// <param name="rows">count of rows from string of parsed matrix</param>
    /// <returns></returns>
    public static List<string> AddEvery(this List<string> input, int every, int rows)
    {
        Parser.ValidArray(input.ToArray(), nameof(input));
        var output = Enumerable.Empty<string>();
        for (var i = 1; i <= rows; i++)
        {
            if (i == 1)
            {
                output = output.Append(input[every * i - 1]);
                continue;
            }

            output = output.Append(input[every * i - 1]);
        }

        return output.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList();
    }

    /// <summary>
    /// get as arg string with/without other symbols like words, punctuations marks etc
    /// return new <see cref="string"/> only with digits
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <returns></returns>
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