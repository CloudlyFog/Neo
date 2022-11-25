using System.Drawing;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace Neo.Services
{
    public class Solver
    {
        public Solver(string path)
        {
            Parser.Input = Reader.Read(path);
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
        }

        public Solver(Image image)
        {
            Parser.Input = Reader.Read(image);
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
        }
        public Matrix<double> LeftSide { get; }
        public Vector<double> RightSide { get; }
        public Vector<double> Result { get; }
    }
}
