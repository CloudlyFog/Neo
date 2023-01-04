using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace TestNeoSoftware;

public class TestEquationParser
{
    private Parser _parser;

    [SetUp]
    public void Setup()
    {
        const string equationInput = "1.3x - 2y + 3z = 4;5x - 6y + 7z = 8;9x + 10y + 11.5z =-  12.5;";
        const string matrixInput = "1.3 -2 3 4;5 -6 7 8;9 10 11.5 -12.5;";
        _parser = new Parser(equationInput);
    }

    [Test]
    public void TestMatrixConversion()
    {
        var correctMatrix = new[,]
        {
            { 1.3d, -2, 3 },
            { 5, -6, 7 },
            { 9, 10, 11.5d },
        };

        var expectedMatrix = Matrix<double>.Build.DenseOfArray(correctMatrix);
        var actualMatrix = _parser.MatrixConversion();

        Assert.AreEqual(expectedMatrix, actualMatrix);
    }

    [Test]
    public void TestVectorConversion()
    {
        var correctVector = new[]
        {
            4, 8, -12.5d,
        };
        var expectedVector = Vector<double>.Build.DenseOfArray(correctVector);
        var actualVector = _parser.VectorConversion();

        Assert.AreEqual(expectedVector, actualVector);
    }
}