using System.Text;
using Android.Widget;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;

namespace NeoSoftware.Utilities
{
    public static class UtilsExtensions
    {
        public static string GetMatrixValue(this Matrix<double> matrix)
        {
            var message = new StringBuilder();
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j <= matrix.ColumnCount; j++)
                {
                    if (j == matrix.ColumnCount)
                    {
                        message = message.Append("\n");
                        continue;
                    }

                    message = message.Append($"{matrix[j, i]}\t\t");
                }
            }

            return message.ToString();
        }

        public static string GetEquations(this GridLayout gridLayout)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < gridLayout.RowCount; i++)
            {
                var child = (EditText)gridLayout.GetChildAt(i);
                sb.Append($"{child.Text}{Parser.SplitSymbol}");
            }

            return sb.ToString();
        }
    }
}