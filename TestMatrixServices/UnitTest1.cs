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
        double[][] secondMatrix =
        {
            new double[] {8181,10026,12627},
            new double[] {18444,22605,28470},
            new double[] {30535,37424,47134}
        };
        
        
        double[][] transposed =
        {
            new double[] {1,4,7},
            new double[] {2,5,8},
            new double[] {3,6,10}
        };

        var expected = new GeneralMatrix(transposed);
        
        var actual = MatrixHighLevel.Transpose(matrix);
        Assert.AreEqual(expected.RowPackedCopy, actual.RowPackedCopy);
    }
}