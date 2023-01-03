using System;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Neo.Services
{
    /// <summary>
    /// class finds unknown variables of equation
    /// </summary>
    public sealed class Solver : IDisposable
    {
        private MatrixParser _matrixParser;
        public static implicit operator string(Solver solver) => solver.ToString();
        public static explicit operator Solver(string input) => new(input);

        public Solver(string input)
        {
            _matrixParser = new MatrixParser(input.Replace("\n", MatrixParser.SplitSymbol.ToString()));
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
            LeftSide = _matrixParser.MatrixConversion();
            RightSide = _matrixParser.VectorConversion();
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
                sb = sb.Append($"x{i + 1}: {Result[i]}\n");
            return sb.ToString();
        }

        private void ReleaseUnmanagedResources()
        {
            LeftSide = null;
            RightSide = null;
            Result = null;
            _matrixParser = null;
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
}