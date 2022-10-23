using MathNet.Numerics.LinearAlgebra;
using System;

namespace Neo.Services
{
    public class MatrixHighLevel
    {

        /// <summary>
        /// returns determinant of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetDeterminant(Matrix<double> matrix) 
            => matrix.Determinant();


        /// <summary>
        /// returns rank of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetRank(Matrix<double> matrix)
            => matrix.Rank();


        /// <summary>
        /// reversing matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> GetReverseMatrix(Matrix<double> matrix)
            => matrix.Inverse();
        
        /// <summary>
        /// Raises the matrix to the specified power
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Matrix<double> Exponentiation(Matrix<double> matrix, int n)
        {
            for (int i = 1; i < n - 1; i++)
                matrix = matrix.Multiply(matrix);
            return matrix;
        }


        /// <summary>
        /// transposing matrix
        /// column go to row and row go to column
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> Transpose(Matrix<double> matrix)
            => matrix.Transpose();
    }
}