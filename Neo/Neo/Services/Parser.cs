using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Neo.Exceptions;

namespace Neo.Services
{
    internal class Parser
    {
        public const char SplitSymbol = ';';

        /// <summary>
        /// output of ocr
        /// </summary>
        private readonly string _input;

        /// <summary>
        /// every what iteration 'll doing something
        /// </summary>
        private int _every = 1;

        public Parser(string input)
        {
            _input = input;
        }

        /// <summary>
        /// Take data from <see cref="_input"/> and put it to <see cref="Matrix{T}"/>
        /// Uses <see cref="_input"/> like 
        /// </summary>
        /// <returns></returns>
        public Matrix<double> ParseToMatrix()
        {
            var targetArray = new double[
                // read count of ";" and therefore count will one less than actually
                _input.Count(x => x == SplitSymbol) + 1,
                // read count of spaces and divide it on count of symbol ";" and subtract 1
                _input.Count(x => x == ' ') / _input.Count(x => x == SplitSymbol) - 1];

            _every = targetArray.GetLength(1);

            // removing white space and commas
            var filterResult = _input.Split(' ', SplitSymbol).Where(x => x != " " || x != string.Empty).ToList()
                .RemoveEvery(_every, targetArray.GetLength(0));

            return GetMatrixValue(targetArray, filterResult);
        }

        /// <summary>
        /// Take data from <see cref="_input"/> and put it to <see cref="Vector{T}"/>
        /// </summary>
        /// <returns></returns>
        public Vector<double> ParseToVector()
        {
            // remove white space and commas
            var filterResult = _input.Split(' ', SplitSymbol).Where(x => x != " " || x != string.Empty).ToList();

            // remove empty space
            filterResult = filterResult.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList()
                .AddEvery(_every, _input.Count(x => x == SplitSymbol) + 1);

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
                throw new ArgumentException($"length of {nameof(targetArray)} less or equals than 0");

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
                    catch (ParserException exception)
                    {
                        Console.WriteLine(exception);
                        throw new ParserException(exception.Message, filterResult[point]);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw new Exception(exception.Message, exception.InnerException);
                    }
                }
            }

            return Matrix<double>.Build.DenseOfArray(targetArray);
        }

        /// <summary>
        /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
        /// </summary>
        /// <param name="targetArray">array which will contain parsed data from <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        private static Vector<double> GetVectorValue(double[] targetArray, List<string> filterResult)
        {
            ValidArray(targetArray);
            // start point for input text as iterator
            var point = 0;
            try
            {
                filterResult =
                    filterResult.TakeWhile(item => ValidIteration(ref point, filterResult)).ToList();
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
            ValidArray(filterResult.ToArray());
            if (point == filterResult.Count)
                return false;
            if (filterResult[point] != string.Empty)
                return true;
            point++;
            return false;
        }

        internal static void ValidArray<T>(T[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length <= 0)
                throw new ArgumentException($"length of {nameof(array)} less or equals than 0");
        }
    }

    public static class ListExtension
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
            Parser.ValidArray(input.ToArray());
            for (var i = 1; i <= rows; i++)
                input.RemoveAt((every - 1) * i - 1);

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
            Parser.ValidArray(input.ToArray());
            var output = new List<string>();
            for (var i = 1; i <= rows; i++)
            {
                if (i == 1)
                {
                    output.Add(input[(every - 1) * i]);
                    continue;
                }

                output.Add(input[every * i - 1]);
            }

            return output.Where(s => !string.IsNullOrWhiteSpace(s)).AsEnumerable().ToList();
        }
    }
}