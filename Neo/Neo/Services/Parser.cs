﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Utilities;

namespace Neo.Services;

public class Parser
{
    /// <summary>
    /// splits equations of system in string
    /// </summary>
    public const char SplitSymbol = ';';

    /// <summary>
    /// determines negative digits
    /// </summary>
    public const char NegativeSymbol = '-';

    /// <summary>
    /// determines float digits with split symbol "." (dot)
    /// </summary>
    public const char FloatSymbolDot = '.';

    /// <summary>
    /// determines float digits with split symbol "," (comma)
    /// </summary>
    public const char FloatSymbolComma = ',';

    /// <summary>
    /// conversed string of system equations
    /// </summary>
    private string _input;

    /// <summary>
    /// every what iteration 'll doing something
    /// </summary>
    private int _every = 1;

    /// <summary>
    /// returns instance of <see cref="Parser"/> with valid value of <see cref="_input"/>
    /// </summary>
    /// <param name="input"><see cref="_input"/></param>
    public Parser(string input)
    {
        _input = input;
    }

    /// <summary>
    /// Converts <see cref="_input"/> to <see cref="Matrix{T}"/>
    /// Uses <see cref="_input"/> like 
    /// </summary>
    /// <param name="isString">defines is entered/passed data for conversion is string</param>
    /// <param name="matrix">matrix if need convert it to view for calculation</param>
    /// <returns></returns>
    public Matrix<double> MatrixConversion(string unknownVariables, Matrix<double> matrix = null)
    {
        if (unknownVariables is null)
            _input = GetStringMatrix(matrix);

        if (_input.ContainsString(unknownVariables))
            _input = _input.GetDigits();

        if (_input is null)
        {
            Error.Message = $"{nameof(_input)} is null.";
            Error.ArgValues = nameof(_input);
            return null;
        }

        var targetArray = new double[
            // read count of ";" and therefore count will one less than actually
            _input.SymbolCount(SplitSymbol),
            // read count of spaces and divide it on count of symbol ";"
            _input.SymbolCount(' ') / _input.SymbolCount(SplitSymbol)];

        _every = targetArray.GetLength(1);

        // removing white space and commas
        var filterResult = _input.Split(' ', SplitSymbol).Where(x => x is not (" " or "")).ToList()
            .RemoveEvery(_every, targetArray.GetLength(0));

        return GetMatrixValue(targetArray, filterResult);
    }

    /// <summary>
    /// Converts <see cref="_input"/> to <see cref="Vector{T}"/>
    /// </summary>
    /// <param name="isString">defines is entered/passed data for conversion is string</param>
    /// <param name="matrix">matrix if need convert it to view for calculation</param>
    /// <returns></returns>
    public Vector<double> VectorConversion(string unknownVariables, Matrix<double> matrix = null)
    {
        if (unknownVariables is null)
            _input = GetStringMatrix(matrix);

        if (_input.ContainsString(unknownVariables))
            _input = _input.GetDigits();

        if (_input is null)
        {
            Error.Message = $"{nameof(_input)} is null.";
            Error.ArgValues = nameof(_input);
            return null;
        }

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
        _every = _input.SymbolCount(' ') / _input.SymbolCount(SplitSymbol) + 1;

        // remove empty space
        filterResult = filterResult.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList()
            // get vector
            .AddEvery(_every, _input.SymbolCount(SplitSymbol));

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
        {
            Error.Message = $"{nameof(targetArray)} is null";
            Error.ArgValues = $"{nameof(targetArray)}: {targetArray}";
            return null;
        }

        if (targetArray.Length <= 0)
        {
            Error.Message = $"length of {nameof(targetArray)} less or equals 0";
            return null;
        }

        if (ValidArray(filterResult.ToArray(), nameof(filterResult)) is null)
            return null;

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
                    Error.Message = exception.Message;
                    Error.InnerMessage = exception.InnerException?.Message;
                    return null;
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
        if (ValidArray(targetArray, nameof(targetArray)) is null ||
            ValidArray(filterResult.ToArray(), nameof(filterResult)) is null)
            return null;

        // start point for input text as iterator
        var point = 0;
        try
        {
            filterResult = filterResult
                .TakeWhile(item => ValidIteration(ref point, filterResult)).ToList();
        }
        catch (Exception exception)
        {
            Error.Message = exception.Message;
            Error.InnerMessage = exception.InnerException?.Message;
            return null;
        }


        for (var i = 0; i < filterResult.Count; i++)
        {
            try
            {
                targetArray[i] = double.Parse(filterResult[i]);
            }
            catch (Exception exception)
            {
                Error.Message = exception.Message;
                Error.InnerMessage = exception.InnerException?.Message;
                return null;
            }
        }

        return Vector<double>.Build.DenseOfArray(targetArray);
    }

    /// <summary>
    /// gets string interpretation of matrix
    /// </summary>
    /// <param name="matrix">original data</param>
    /// <returns></returns>
    private static string GetMatrixValue(Matrix<double> matrix)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < matrix.ColumnCount; i++)
        {
            for (var j = 0; j < matrix.RowCount; j++)
                sb.Append($"{matrix[j, i]} ");
            sb.Append("\n");
        }

        return sb.ToString();
    }

    /// <summary>
    /// gets converted matrix to string
    /// </summary>
    /// <param name="matrix">original data</param>
    /// <returns></returns>
    private static string GetStringMatrix(Matrix<double> matrix)
    {
        return ValidArray(matrix.ToArray(), nameof(matrix)) is null
            ? null
            : GetMatrixValue(matrix).Replace('\n', SplitSymbol).RemoveWhiteSpacesNearSeparator();
    }

    /// <summary>
    /// valid passed two dimension array for the some specifications
    /// </summary>
    /// <param name="array">passed array</param>
    /// <param name="arrayName">name of passed array (name of variable)</param>
    /// <typeparam name="T">the type of elements in the array</typeparam>
    /// <returns></returns>
    private static Error ValidArray<T>(T[,] array, string arrayName)
    {
        if (array is null)
        {
            Error.Message = $"{arrayName} is null.";
            return null;
        }

        if (array.Length <= 0)
        {
            Error.Message = $"length of {arrayName} less or equals 0";
            return null;
        }

        return new Error();
    }

    /// <summary>
    /// valid passed array for the some specifications
    /// </summary>
    /// <param name="array">passed array</param>
    /// <param name="arrayName">name of passed array (name of variable)</param>
    /// <typeparam name="T">the type of elements in the array</typeparam>
    /// <returns></returns>
    internal static Error ValidArray<T>(T[] array, string arrayName)
    {
        if (array is null)
        {
            Error.Message = $"{arrayName} is null.";
            return null;
        }

        if (array.Length <= 0)
        {
            Error.Message = $"length of {arrayName} less or equals 0";
            return null;
        }

        return new Error();
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
}