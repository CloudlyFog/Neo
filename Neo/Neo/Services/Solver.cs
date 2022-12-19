using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Xamarin.Forms.Internals;

namespace Neo.Services
{
    public sealed class Solver : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _solveInsideConstructor;
        private Parser _parser;
        private Solver _solver;

        public Solver(Stream stream, bool solveInsideConstructor = false)
        {
            _stream = stream;
            _solveInsideConstructor = solveInsideConstructor;
            if (solveInsideConstructor)
                _solver = Read();
        }

        private Solver(Matrix<double> leftSide, Vector<double> rightSide, Vector<double> result)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
            Result = result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<Solver> ReadAsync()
        {
            if (_solveInsideConstructor)
                throw new ArgumentException("you can't run this method because you already read in constructor." +
                                            "\nplease don't specify true value for solveInsideConstructor");
            if (_stream is null)
                throw new ArgumentException("passed to class constructor {stream} is null.");
            try
            {
                using var reader = new Reader(_stream);
                var readerOutput = await reader.ReadAsync();

                if (readerOutput.Equals(string.Empty) || readerOutput is null)
                    throw new ArgumentException("output of the {reader} is empty string." +
                                                "\nPlease check on valid passed to class " +
                                                "constructor arg {stream}.");
                _parser = new Parser(readerOutput);

                Solve();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            finally
            {
                Dispose();
            }

            return new Solver(LeftSide, RightSide, Result);
        }

        private Solver Read()
        {
            if (_stream is null)
                throw new ArgumentException("passed to class constructor {stream} is null.");
            try
            {
                using var reader = new Reader(_stream);
                var readerOutput = reader.Read();
                if (readerOutput.Equals(string.Empty) || readerOutput is null)
                    throw new ArgumentException("output of the {reader} is empty string." +
                                                "\nPlease check on valid passed to class " +
                                                "constructor arg {stream}.");
                _parser = new Parser(readerOutput);

                Solve();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new InvalidOperationException(exception.Message, exception.InnerException);
            }
            finally
            {
                Dispose();
            }

            return new Solver(LeftSide, RightSide, Result);
        }

        private void Solve()
        {
            LeftSide = _parser.ParseToMatrix();
            RightSide = _parser.ParseToVector();
            try
            {
                Result = LeftSide.Solve(RightSide);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new InvalidOperationException(exception.Message, exception.InnerException);
            }
            finally
            {
                Dispose();
            }
        }

        public Matrix<double> LeftSide { get; private set; }
        public Vector<double> RightSide { get; private set; }
        public Vector<double> Result { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var d in Result)
                sb = sb.Append($"{Result.IndexOf(d)}: {d}\n");
            return sb.ToString();
        }

        private void ReleaseUnmanagedResources()
        {
            LeftSide = null;
            RightSide = null;
            Result = null;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _stream?.Dispose();
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