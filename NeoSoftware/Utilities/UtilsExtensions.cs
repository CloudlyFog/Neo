using System.Text;
using Android.Widget;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using Neo.Utilities;

namespace NeoSoftware.Utilities
{
    public static class UtilsExtensions
    {
        public static string GetMatrixValue(this Matrix<double> matrix)
        {
            var message = new StringBuilder();
            for (var i = 0; i < matrix.ColumnCount; i++)
            {
                for (var j = 0; j < matrix.RowCount; j++)
                    message.Append($"{matrix[j, i]}\t\t");
                message.Append("\n");
            }

            return message.ToString();
        }

        public static string GetMatrixValue(this GridLayout gridLayout)
        {
            var sb = new StringBuilder();
            var index = 0;
            var wrongValuesCounter = 1;
            for (var i = 0; i < gridLayout.RowCount; i++)
            {
                for (var j = 0; j < gridLayout.ColumnCount; j++, index++)
                {
                    var child = (EditText)gridLayout.GetChildAt(index);
                    if (child.Text != string.Empty && !int.TryParse(child.Text, out var result))
                        sb.Append($"{wrongValuesCounter++}) {{\"{child.Text}\"}} at [{i};{j}]\n");
                }
            }

            return sb.ToString();
        }

        public static string GetEquations(this GridLayout gridLayout, char splitSymbol = Parser.SplitSymbol)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < gridLayout.RowCount; i++)
            {
                var child = (EditText)gridLayout.GetChildAt(i);
                sb.Append($"{child.Text}{splitSymbol}");
            }

            return sb.ToString();
        }

        public static bool IsSquare(this Matrix<double> matrix)
        {
            if (matrix.RowCount == matrix.ColumnCount)
                return true;
            Error.Message = "matrix must be square.";
            return false;
        }

        public static bool IsGaussianMatrix(this Matrix<double> matrix)
        {
            if (matrix.RowCount == matrix.ColumnCount + 1)
                return true;
            Error.Message = "matrix can't solve by Gaussian method due to incorrect size of its sides.";
            return false;
        }
    }
}