using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace TestNeoSoftware;

public class Tests
{
    private Parser _parser;

    [SetUp]
    public void Setup()
    {
        _parser = new Parser("1 2 3 4;5 6 7 8;9 10 11 12;");
    }

    [Test]
    public void TestMatrixSizeCalculating()
    {
        var correctMatrix = new double[,]
        {
            { 1, 2, 3 },
            { 5, 6, 7 },
            { 9, 10, 11 },
        };

        var expectedMatrix = Matrix<double>.Build.DenseOfArray(correctMatrix);
        var actualMatrix = _parser.MatrixConversion();

        Assert.AreEqual(expectedMatrix, actualMatrix);
    }

    [Test]
    public void TestVectorSizeCalculating()
    {
        var correctVector = new double[]
        {
            4, 8, 12,
        };
        var expectedVector = Vector<double>.Build.DenseOfArray(correctVector);
        var actualVector = _parser.VectorConversion();

        Assert.AreEqual(expectedVector, actualVector);
    }
}