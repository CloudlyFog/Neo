using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace Neo.Services
{
    public class Solver
    {
        public Solver(string input)
        {
            Parser.Input = input;
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
        }
        public Matrix<double> LeftSide { get; }
        public Vector<double> RightSide { get; }
        public Vector<double> Result { get; }
    }
}
