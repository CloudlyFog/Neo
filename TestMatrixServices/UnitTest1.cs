using DotNetMatrix;
using NeoMatrixServices.Services;

namespace TestMatrixServices;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestDeterminant()
    {
        var expected = 3;
        double[][] matrix =
        {
            new double[] {1,2,3},
            new double[] {4,5,6},
            new double[] {7,8,10}
        };
        var actual = MatrixHighLevel.GetRank(matrix);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void Matrix()
    {
        double[][] matrix =
        {
            new double[] {1,2,3},
            new double[] {4,5,6},
            new double[] {7,8,10}
        };
        double[][] revMatrix =
        {
            new double[] {1,2,3},
            new double[] {0,1,2},
            new double[] {0,0,1}
        };
        var expected = new GeneralMatrix(revMatrix);
        var actual = MatrixHighLevel.GetReverseMatrix(matrix);
        Assert.AreEqual(expected, actual);
    }
}