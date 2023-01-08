using System;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Utilities;

namespace Neo.Services;

/// <summary>
/// class finds unknown variables of equation
/// </summary>
public sealed class Solver : IDisposable
{
    private readonly string _input;
    private Parser _parser;

    public static implicit operator string(Solver solver) => solver.ToString();
    public static implicit operator Vector<double>(Solver solver) => solver.Result;
    public static explicit operator Solver(string input) => new(input);

    public Solver(string input)
    {
        _input = input.Replace("\n", Parser.SplitSymbol.ToString()).ToLower();

        if (_input is "no text" or "" || _input.IsTrash())
        {
            Error.Message = "didn't read anything.";
            Error.ArgValues = input;
            return;
        }

        _parser = new Parser(_input);
        try
        {
            Solve();
        }
        catch (Exception exception)
        {
            Error.Message = exception.Message;
            Error.InnerMessage = exception.InnerException?.Message;
        }
    }

    private void Solve()
    {
        LeftSide = _parser.MatrixConversion();
        if (LeftSide is null)
        {
            Error.Message = $"{nameof(LeftSide)} is null.";
            Dispose();
            return;
        }

        RightSide = _parser.VectorConversion();
        if (RightSide is null)
        {
            Error.Message = $"{nameof(RightSide)} is null.";
            Dispose();
            return;
        }

        try
        {
            Result = LeftSide.Solve(RightSide);
        }
        catch (Exception exception)
        {
            Error.Message = exception.Message;
            Error.InnerMessage = exception.InnerException?.Message;
            Dispose();
        }
    }

    /// <summary>
    /// describe left side (matrix) of equation
    /// </summary>
    public Matrix<double> LeftSide { get; private set; }

    /// <summary>
    /// describe right side (results) of equation
    /// </summary>
    public Vector<double> RightSide { get; private set; }

    /// <summary>
    /// describe results of unknown variables of system linear equations
    /// </summary>
    public Vector<double> Result { get; private set; }

    public override string ToString()
    {
        if (Error.Message is not null)
            return $"{Error.Message}\n{Error.InnerMessage}\n{Error.ArgValues}";

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