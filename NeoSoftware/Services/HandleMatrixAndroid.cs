using Android.Widget;
using MathNet.Numerics.LinearAlgebra;

namespace NeoSoftware.Services
{
    public abstract class HandleMatrixAndroid
    {
        /// <summary>
        /// converting data from IGridListView to double[,]
        /// </summary>
        /// <returns></returns>
        public static Matrix<double> GetMatrix(GridLayout gridLayout, int rows, int columns)
        {
            var matrix = new double[columns, rows];
            var startPoint = 0; // like an i in default cycle
            var j = 0;
            for (var i = 0; startPoint < gridLayout.ChildCount; i++)
            {
                if (ValidateIterators(ref i, ref j, rows, columns))
                    break;
                matrix = SetMatrix(matrix, gridLayout, startPoint, i, j);
                startPoint++;
            }

            return Matrix<double>.Build.DenseOfArray(matrix);
        }

        private static bool ValidateIterators(ref int i, ref int j, int rows, int columns)
        {
            // if i equals count of columns
            // we go to the next row and resetting to zero i
            if (i == columns)
            {
                i = 0;
                j++;
            }

            // if j equals count of columns
            // we stopping all
            if (j == rows)
                j = 0;
            return j == 0;
        }

        private static double[,] SetMatrix(double[,] matrix, GridLayout gridLayout, int startPoint, int i, int j)
        {
            // get frame with index startPoint from array of MatrixGrid
            var child = (TextView)gridLayout.GetChildAt(startPoint);

            // assign value of entered Entry
            matrix[i, j] = double.Parse(child.Text);
            return matrix;
        }
    }
}