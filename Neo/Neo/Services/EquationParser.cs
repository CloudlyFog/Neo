using System.Text;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using AngouriMath;
using AngouriMath.Extensions;

namespace Neo.Services;

public sealed class EquationParser
{
    private readonly MatrixParser _matrixParser;

    public EquationParser(string input)
    {
        _matrixParser = new MatrixParser(input.GetDigits());
    }

    public Matrix<double> MatrixConversion() => _matrixParser.MatrixConversion();

    public Vector<double> VectorConversion() => _matrixParser.VectorConversion();
}

public static partial class ListExtension
{
    public static string GetDigits(this string input)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]) || input[i] == MatrixParser.SplitSymbol)
                sb.Append(input[i]);
            else
                sb.Append(' ');
        }

        Entity expr = "2 / 5 + 6";


        return sb.ToString();
    }
}