using NeoMatrixServices.Services;

namespace TestMatrixServices;

public class Tests
{
    private Determinant _determinant;
    [SetUp]
    public void Setup()
    {
        _determinant = new();
    }

    [Test]
    public void TestDeterminant()
    {
        var expected = -5;
        var matrix = new double[,]
        {
            { 3, 3, 4 },
            { 1, 2, 4 },
            { 3, 4, 5 }
        };
        var actual = Determinant.Order3(matrix);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestSize()
    {
        var expected = 3;
        var matrix = new double[,]
        {
            { 3, 3, 4 },
            { 1, 2, 4 },
            { 3, 4, 5 }
        };
        var actual1 = Determinant.EnoughSize(matrix, 1);
        var actual2 = Determinant.EnoughSize(matrix, 2);
        var actual3 = Determinant.EnoughSize(matrix, 3);
        var actual4 = Determinant.EnoughSize(matrix, 4);
        var actual5 = Determinant.EnoughSize(matrix, 5);
        Assert.AreEqual(true, actual1);
        Assert.AreEqual(true, actual2);
        Assert.AreEqual(true, actual3);
        Assert.AreEqual(false, actual4);
        Assert.AreEqual(false, actual5);
    }
}