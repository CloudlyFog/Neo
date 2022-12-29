using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Neo.Services
{
    /// <summary>
    /// This class can solve only square equations and matrix
    /// </summary>
    internal class MatrixEquation
    {
        public const char SplitSymbol = ';';
        public Vector<double> RightSide { get; set; }
        public Matrix<double> LeftSide { get; set; }

        /// <summary>
        /// parse data from Tesseract OCR and convert it to MatrixEquation
        /// </summary>
        /// <param name="input">received data from Tesseract</param>
        /// <returns></returns>
        public static MatrixEquation GetMatrixEquation(string input)
        {
            var rightSideMatrix = new StringBuilder();
            var leftSideMatrix = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                var point = i;
                if (++point >= input.Length)
                    break;
                if (input[point] == SplitSymbol)
                    rightSideMatrix = rightSideMatrix.Append($"{input[i]}{SplitSymbol}");
                else
                    leftSideMatrix = leftSideMatrix.Append(input[i]);
            }

            var matrix = new MatrixEquation()
            {
                LeftSide = GetLeftSide(leftSideMatrix.ToString()),
                RightSide = GetRightSide(rightSideMatrix.ToString())
            };
            return matrix;
        }

        /// <summary>
        /// gets right side of matrix
        /// </summary>
        /// <param name="input">received data from Tesseract</param>
        /// <returns></returns>
        private static Vector<double> GetRightSide(string input)
        {
            var vector = new double[
                input.Count(x => x == SplitSymbol) + 1];
            var point = 0;
            foreach (var item in input.Where(item => item != SplitSymbol))
                vector[point++] = double.Parse(item.ToString());
            return Vector<double>.Build.DenseOfArray(vector);
        }

        /// <summary>
        /// gets left side of matrix
        /// </summary>
        /// <param name="input">received data from Tesseract</param>
        /// <returns></returns>
        private static Matrix<double> GetLeftSide(string input)
        {
            var targetArray = new double[
                // set count of columns by parsing input text
                // at the end of any matrix's row will types comma
                input.Count(x => x == SplitSymbol),

                // takes all elements before comma
                input.TakeWhile(x => x != SplitSymbol).Count(x => !char.IsWhiteSpace(x)) - 2];


            // removing white space and commas
            var filterResult = input.Split(' ', SplitSymbol).Where(x => x != " " || x != string.Empty).ToList();

            // removing empty space
            filterResult.RemoveAll(string.IsNullOrWhiteSpace);
            return Matrix<double>.Build.DenseOfArray(AddToMatrix(targetArray, filterResult));
        }

        /// <summary>
        /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
        /// </summary>
        /// <param name="targetArray">array which will contain parsed data from <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        private static double[,] AddToMatrix(double[,] targetArray, List<string> filterResult)
        {
            // start point for input text
            // like an iterator
            var point = 0;

            for (var i = 0; i < targetArray.GetLength(0);)
            {
                // on every iteration we increasing point instead of j
                // because we have to iterate filterResult but not columns of sourceArray
                for (var j = 0; j < targetArray.GetLength(1); point++)
                {
                    targetArray[i, j] = double.Parse(filterResult[point]);
                    j++;
                }

                i++;
            }

            return targetArray;
        }
    }
}