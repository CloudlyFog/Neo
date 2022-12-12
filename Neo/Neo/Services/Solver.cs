using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSoftware.Drawing;
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

        public Solver(AnyBitmap anyBitmap)
        {
            Parser.Input = Reader.Read(anyBitmap);
            OnCreating();
        }

        public Solver()
        {
            
        }

        public async Task<Vector<decimal>> ReadAsync(AnyBitmap anyBitmap)
        {
            Parser.Input = await Reader.ReadAsync(anyBitmap);
            return Parser.Input.Equals(string.Empty) ? null : OnSolving();
        }

        private void OnCreating()
        {
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
        }

        private Vector<decimal> OnSolving()
        {
            LeftSide = Parser.ParseToMatrix();
            RightSide = Parser.ParseToVector();
            Result = LeftSide.Solve(RightSide);
            return Result;
        }

        public Matrix<decimal> LeftSide { get; private set; }
        public Vector<decimal> RightSide { get; private set; }
        public Vector<decimal> Result { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb = Result.Aggregate(sb, (current, t) => current.Append(t));
            return sb.ToString();
        }
    }
}
