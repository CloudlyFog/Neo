using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace TestNeoSoftware;

public class TestEquationParser
{
    private EquationParser _equationParser;

    [SetUp]
    public void Setup()
    {
        var equationInput = "1x + 2y + 3z = 4;5x + 6y + 7z = 8;9x + 10y + 11z = 12;";
        const string matrixInput = "1 2 3 4;5 6 7 8;9 10 11 12;";
        _equationParser = new EquationParser(matrixInput);
    }

    [Test]
    public void TestMatrixConversion()
    {
        var correctMatrix = new double[,]
        {
            { 1, 2, 3 },
            { 5, 6, 7 },
            { 9, 10, 11 },
        };

        var expectedMatrix = Matrix<double>.Build.DenseOfArray(correctMatrix);
        var actualMatrix = _equationParser.MatrixConversion();

        Assert.AreEqual(expectedMatrix, actualMatrix);
    }

    [Test]
    public void TestVectorConversion()
    {
        var correctVector = new double[]
        {
            4, 8, 12,
        };
        var expectedVector = Vector<double>.Build.DenseOfArray(correctVector);
        var actualVector = _equationParser.VectorConversion();

        Assert.AreEqual(expectedVector, actualVector);
    }
}