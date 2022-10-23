using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Neo.Services
{
    public class MatrixBasicLevel
    {
        public static Matrix<double> Add(Matrix<double> matrix1, Matrix<double> matrix2)
            => matrix1.Add(matrix2);

        public static Matrix<double> Subtract(Matrix<double> matrix1, Matrix<double> matrix2)
            => matrix1.Subtract(matrix2);
    
        public static Matrix<double> Multiply(Matrix<double> matrix1, Matrix<double> matrix2)
            => matrix1.Multiply(matrix2);
    }
}