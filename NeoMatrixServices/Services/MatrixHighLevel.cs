
using MathNet.Numerics.LinearAlgebra;
using DotNetMatrix;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;

namespace NeoMatrixServices.Services;

public class MatrixHighLevel
{
    
    /// <summary>
    /// returns determinant of matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static double GetDeterminant(double[][] matrix) 
        => new GeneralMatrix(matrix).Determinant();

    
    /// <summary>
    /// returns rank of matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static double GetRank(double[][] matrix) 
        => new GeneralMatrix(matrix).Rank();

    
    /// <summary>
    /// reversing matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static GeneralMatrix GetReverseMatrix(double[][] matrix) 
        => new GeneralMatrix(matrix).Inverse();
    
    /// <summary>
    /// Raises the matrix to the specified power
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    public static GeneralMatrix Exponentiation(double[][] matrix, int n)
    {
        var expMatrix = new GeneralMatrix(matrix);
        for (int i = 1; i < n - 1; i++)
            expMatrix = expMatrix.Multiply(expMatrix);
        return expMatrix;
    }

    
    /// <summary>
    /// transposing matrix
    /// column go to row and row go to column
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static GeneralMatrix Transpose(double[][] matrix) 
        => new GeneralMatrix(matrix).Transpose();
}