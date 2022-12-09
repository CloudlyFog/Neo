using System.Drawing;
using System.IO;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace Neo.Services
{
    public sealed class Solver
    {
        public Solver(string path)
        {
            Parser.Input = Reader.Read(path);
            OnCreating();
        }

        public Solver(Image image)
        {
            Parser.Input = Reader.Read(image);
            OnCreating();
        }

        public Solver(Stream stream)
        {
            Parser.Input = Reader.Read(stream);
            OnCreating();
        }

        public Solver(byte[] bytes)
        {
            Parser.Input = Reader.Read(bytes);
            OnCreating();
        }

        private void OnCreating()
        {
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
        }
        
        public Matrix<double> LeftSide { get; private set; }
        public Vector<double> RightSide { get; private set; }
        public Vector<double> Result { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Result.Count; i++)
                sb = sb.Append(Result[i]);
            return sb.ToString();
        }
    }
}
