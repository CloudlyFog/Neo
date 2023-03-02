using System;
using MathNet.Numerics.LinearAlgebra;
using Neo.Utilities;

namespace Neo.Services
{
    public static class MatrixHighLevel
    {
        /// <summary>
        /// returns determinant of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetDeterminant(this Matrix<double> matrix)
            => matrix.Determinant();

        /// <summary>
        /// returns rank of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetRank(this Matrix<double> matrix)
            => matrix.Rank();

        /// <summary>
        /// reversing matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> GetReverseMatrix(this Matrix<double> matrix)
            => matrix.Inverse();

        /// <summary>
        /// Raises the matrix to the specified power
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="n">Exponentiation value</param>
        /// <returns></returns>
        public static Matrix<double> Exponentiation(this Matrix<double> matrix, int n)
        {
            for (var i = 1; i < n; i++)
                matrix = matrix.Multiply(matrix);

            return matrix;
        }

        /// <summary>
        /// transposing matrix
        /// column go to row and row go to column
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> Transpose(this Matrix<double> matrix)
            => matrix.Transpose();

        public static bool IsSymmetrical(this Matrix<double> matrix)
        {
            if (matrix.RowCount == matrix.ColumnCount)
                return true;
            Error.Message = $"matrix isn't symmetric.";
            return false;
        }
    }
}