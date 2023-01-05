using Neo.Services;

namespace TestNeoSoftware;

public class TestSolver
{
    private Parser _parser;
    const string equationInput = "1.3x - 2y + 3z = 4;5x - 6y + 7z = 8;9x + 10y + 11.5z =-  12.5;";
    const string matrixInput = "1.3 -2 3 4;5 -6 7 8;9 10 11.5 -12.5;";

    [SetUp]
    public void Setup()
    {
        _parser = new Parser(equationInput);
    }

    [Test]
    public void TestGetUnknownVariables()
    {
        var expected = "xyz";
        var actual = equationInput.GetUnknownVariables();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void TestSolverToString()
    {
        var expected = "x: -1.5128844555278471\r\ny: -1.2315045719035744\r\nz: 1.167913549459684\r\n";
        string actual = new Solver(equationInput);

        Assert.AreEqual(expected, actual);
    }
}