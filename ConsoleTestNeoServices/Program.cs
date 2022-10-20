using System.Threading.Channels;
using DotNetMatrix;

double[][] matrix =
{
    new double[] {1,2,3},
    new double[] {4,5,6},
    new double[] {7,8,10}
};

var generalMatrix = new GeneralMatrix(matrix);
var n = 4;

var result = Exp(generalMatrix, n);
Console.WriteLine();

GeneralMatrix Exp(GeneralMatrix expMatrix, int n)
{
    for (int i = 1; i < n - 1; i++)
    {
        var mult = expMatrix.Multiply(expMatrix);
        expMatrix = expMatrix.Multiply(expMatrix);
    }

    return expMatrix;
}
