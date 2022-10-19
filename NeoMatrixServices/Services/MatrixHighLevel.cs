

using DotNetMatrix;

namespace NeoMatrixServices.Services;

public class MatrixHighLevel
{

    public static double GetDeterminant(double[][] matrix) 
        => new GeneralMatrix(matrix).Determinant();

    public static double GetRank(double[][] matrix) 
        => new GeneralMatrix(matrix).Rank();

    public static GeneralMatrix GetReverseMatrix(double[][] matrix) 
        => new GeneralMatrix(matrix).Inverse();
}