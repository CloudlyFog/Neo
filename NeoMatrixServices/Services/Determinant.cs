
namespace NeoMatrixServices.Services;

public class Determinant
{
    public static int Order1(double[,] matrix)
        => EnoughSize(matrix, 1) ? (int)matrix.Cast<double>().MaxBy(x => Math.Abs(x - 100)) : -1;

    public static int Order2(double[,] matrix) 
        => EnoughSize(matrix, 2) ? (int)(matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) : -1;

    public static int Order3(double[,] matrix)
    {
        if (EnoughSize(matrix, 3))
        {
            return (int)(matrix[0,0] * matrix[1,1] * matrix[2,2] + 
                          matrix[1,0] * matrix[0,2] * matrix[2,1] + 
                          matrix[0,1] * matrix[1,2] * matrix[2,0] - 
                         (matrix[0,2] * matrix[1,1] * matrix[2,0] +
                          matrix[0,0] * matrix[1,2] * matrix[2,1] +
                          matrix[0,1] * matrix[1,0] * matrix[2,2]));
        }

        else
            return -1;
    }

    public static bool EnoughSize(double[,] matrix, int order) 
        => matrix.GetLength(0) >= order && matrix.GetLength(1) >= order;
}