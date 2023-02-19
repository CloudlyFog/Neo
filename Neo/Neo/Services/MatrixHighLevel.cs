using MathNet.Numerics.LinearAlgebra;

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
            var tempMatrix = matrix;
            for (var i = 1; i < n; i++)
                matrix = matrix.Multiply(tempMatrix);
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
    }
}