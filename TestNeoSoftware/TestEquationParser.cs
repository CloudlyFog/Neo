using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using Neo.Utilities;
using NUnit.Framework.Interfaces;

namespace TestNeoSoftware;

public class TestEquationParser
{
    private Parser _parser;
    const string equationInput = "x - 2y + 3z = 4;5x - 6y + z = 8;9x + y + 11.5z =-  12.5;";
    const string matrixInput = "1.3 -2 3 4;5 -6 7 8;9 10 11.5 -12.5;";

    [SetUp]
    public void Setup()
    {
        //_parser = new Parser(equationInput);
    }

    [Test]
    public void TestMatrixConversion()
    {
        var correctMatrix = new[,]
        {
            { 1d, -2, 3 },
            { 5, -6, 1 },
            { 9, 1, 11.5d },
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

    [Test]
    public void TestOnUnitVariable()
    {
        var correctMatrix = new[,]
        {
            { 1d, -2, 3 },
            { 5, -6, 1 },
            { 9, 1, 11.5d },
        };
        var expected = Matrix<double>.Build.DenseOfArray(correctMatrix);
        var actual = new Parser(equationInput);

        Assert.AreEqual(expected, actual.MatrixConversion());
    }

    [Test]
    public void TestIsTrash()
    {
        var input = new List<string>
        {
            "f21fe11wffd",
            "fff111",
            equationInput,
        };

        var expected = new List<bool>
        {
            true,
            true,
            false,
        };

        var actual = new List<bool>
        {
            input[0].IsTrash(),
            input[1].IsTrash(),
            input[2].IsTrash(),
        };

        Assert.Multiple(() =>
        {
            Assert.That(expected[0], Is.EqualTo(actual[0]));
            Assert.That(expected[1], Is.EqualTo(actual[1]));
            Assert.That(expected[2], Is.EqualTo(actual[2]));
        });
    }
    
}