using System;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Neo.Services;

/// <summary>
/// class finds unknown variables of equation
/// </summary>
public sealed class Solver : IDisposable
{
    private readonly string _input;
    private Parser _parser;

    public static string Input { get; private set; }

    public static implicit operator string(Solver solver) => solver.ToString();
    public static explicit operator Solver(string input) => new(input);

    public Solver(string input)
    {
        _input = input.Replace("\n", Parser.SplitSymbol.ToString()).ToLower();
        Input = _input;
        _parser = new Parser(_input);
        try
        {
            Solve();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void Solve()
    {
        LeftSide = _parser.MatrixConversion();
        RightSide = _parser.VectorConversion();
        try
        {
            Result = LeftSide.Solve(RightSide);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dispose();
            throw new InvalidOperationException(exception.Message, exception.InnerException);
        }
    }

    public Matrix<double> LeftSide { get; private set; }
    public Vector<double> RightSide { get; private set; }
    public Vector<double> Result { get; private set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Result.Count; i++)
            sb.AppendLine($"{_input.GetUnknownVariables()[i]}: {Result[i]}");

        return sb.ToString();
    }

    private void ReleaseUnmanagedResources()
    {
        LeftSide = null;
        RightSide = null;
        Result = null;
        _parser = null;
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Solver()
    {
        Dispose(false);
    }
}

public static partial class ListExtension
{
    /// <summary>
    /// return string with unknown variables from system linear equations
    /// </summary>
    /// <param name="input">parsed string (expected from <see cref="Matrix{T}"/>)</param>
    /// <returns></returns>
    public static string GetUnknownVariables(this string input)
    {
        return new string(input.Where(char.IsLetter).Distinct().ToArray());
    }
}