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
        var expected = 3;
        var matrix = new double[,]
        {
            { 3, 3, 4 },
            { 1, 2, 4 },
            { 3, 4, 5 }
        };
        var actual = Determinant.Order2(matrix);
        Assert.AreEqual(expected, actual);
    }
}