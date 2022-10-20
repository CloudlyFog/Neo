using DotNetMatrix;

namespace NeoMatrixServices.Services;

public class MatrixBasicLevel
{
    public static GeneralMatrix Add(double[][] matrix1, double[][] matrix2) 
        => new GeneralMatrix(matrix1).Add(new GeneralMatrix(matrix2));

    public static GeneralMatrix Subtract(double[][] matrix1, double[][] matrix2)
        => new GeneralMatrix(matrix1).Subtract(new GeneralMatrix(matrix2));
    
    public static GeneralMatrix Multiply(double[][] matrix1, double[][] matrix2)
        => new GeneralMatrix(matrix1).Multiply(new GeneralMatrix(matrix2));
}