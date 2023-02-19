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

        public static string GetMatrixValues(this GridLayout gridLayout, char splitSymbol = Parser.SplitSymbol)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < gridLayout.RowCount; i++)
            {
                for (var j = 0; j < gridLayout.ColumnCount; j++)
                {
                    var child = (EditText)gridLayout.GetChildAt(i + j);
                    var text = child.Text != string.Empty ? child.Text : "@";
                    sb.Append($"{text}{splitSymbol}");
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}