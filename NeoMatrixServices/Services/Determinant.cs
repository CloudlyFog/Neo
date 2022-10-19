
namespace NeoMatrixServices.Services;

public class Determinant
{
    public static int Order1(double[,] matrix) 
        => (int)matrix.Cast<double>().MaxBy(x => Math.Abs(x - 100));

    public static int Order2(double[,] matrix) 
        => (int)(matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]);
}