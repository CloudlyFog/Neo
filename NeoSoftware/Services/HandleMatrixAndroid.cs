using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using MathNet.Numerics.LinearAlgebra;
using NeoSoftware.Utilities;
using Error = Neo.Utilities.Error;
using Exception = System.Exception;

namespace NeoSoftware.Services
{
    public static class HandleMatrixAndroid
    {
        /// <summary>
        /// converting data from IGridListView to double[,]
        /// </summary>
        /// <param name="gridLayout">current grid of ui</param>
        /// <returns></returns>
        public static Matrix<double> GetMatrix(this GridLayout gridLayout)
        {
            if (gridLayout is null)
            {
                Error.Message = $"{nameof(gridLayout)} is null";
                return null;
            }

            var matrix = new double[gridLayout.ColumnCount, gridLayout.RowCount];
            var startPoint = 0; // like an i in default cycle
            var j = 0;
            for (var i = 0; startPoint < gridLayout.ChildCount; i++)
            {
                if (ValidateIterators(ref i, ref j, gridLayout.RowCount, gridLayout.ColumnCount))
                    break;
                matrix = SetMatrix(matrix, gridLayout, startPoint, i, j);
                if (matrix is null)
                    return null;
                startPoint++;
            }

            return Matrix<double>.Build.DenseOfArray(matrix);
        }

        /// <summary>
        /// gets <see cref="GridLayout"/> with values of children from passed matrix"/>
        /// </summary>
        /// <param name="matrix">matrix with current values</param>
        /// <param name="oldGridLayout">current <see cref="GridLayout"/> to which children will adds values</param>
        /// <param name="context">context of app for adding children to <see cref="oldGridLayout"/></param>
        /// <returns></returns>
        public static GridLayout GetGridLayout(this Matrix<double> matrix, GridLayout oldGridLayout, Context context)
        {
            if (!ValidObtainingGridLayout(matrix, oldGridLayout, context))
                return null;
            if (!ValidMatrixValues(oldGridLayout))
                return null;

            oldGridLayout.RemoveAllViews();
            var index = 0;
            for (var i = 0; i < oldGridLayout.RowCount; i++)
            {
                for (var j = 0; j < oldGridLayout.ColumnCount; j++, index++)
                {
                    var child = new EditText(context)
                    {
                        Text = matrix[j, i].ToString(),
                        TextSize = 18,
                        TextAlignment = TextAlignment.Center,
                    };
                    child.SetWidth(150);
                    child.SetHeight(100);
                    oldGridLayout.AddView(child, index);
                }
            }

            return oldGridLayout;
        }

        /// <summary>
        /// validates passed values of args 
        /// </summary>
        /// <param name="matrix">matrix with current values</param>
        /// <param name="oldGridLayout">current <see cref="GridLayout"/> to which children will adds values</param>
        /// <param name="context">context of app for adding children to <see cref="oldGridLayout"/></param>
        /// <returns></returns>
        private static bool ValidObtainingGridLayout(Matrix<double> matrix, GridLayout oldGridLayout, Context context)
        {
            if (matrix is null)
            {
                Error.Message = $"{nameof(matrix)} is null";
                return false;
            }

            if (oldGridLayout is null)
            {
                Error.Message = $"{nameof(oldGridLayout)} is null";
                return false;
            }

            if (context is null)
            {
                Error.Message = $"{nameof(context)} is null";
                return false;
            }

            return true;
        }

        /// <summary>
        /// validates passed indices
        /// </summary>
        /// <param name="i">referenced value of i (index of rows)</param>
        /// <param name="j">referenced value of j (index of columns)</param>
        /// <param name="rows">count of rows</param>
        /// <param name="columns">count of columns</param>
        /// <returns></returns>
        private static bool ValidateIterators(ref int i, ref int j, int rows, int columns)
        {
            // if i equals count of columns
            // we go to the next row and resetting to zero i
            if (i == 0)
                return false;

            if (i != columns)
                return j == rows;
            i = 0;
            j++;

            // if j equals count of columns
            // we stopping all
            return j == rows;
        }

        /// <summary>
        /// validates values of grid's children
        /// </summary>
        /// <param name="gridLayout">current grid of ui</param>
        /// <returns></returns>
        private static bool ValidMatrixValues(GridLayout? gridLayout)
        {
            for (var i = 0; i < gridLayout.ChildCount; i++)
            {
                var child = (EditText)gridLayout.GetChildAt(i);
                if (child.Text.Any(char.IsDigit))
                {
                    Error.Message = "values of matrix contains non digit symbols. Please check entered values.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// sets values from <see cref="GridLayout"/>'s children to matrix
        /// </summary>
        /// <param name="matrix">matrix where adds values</param>
        /// <param name="gridLayout">grid from which takes values to add to matrix</param>
        /// <param name="startPoint">index of <see cref="GridLayout"/>'s children</param>
        /// <param name="i">index of matrix's rows</param>
        /// <param name="j">index of matrix's columns</param>
        /// <returns></returns>
        private static double[,] SetMatrix(double[,] matrix, GridLayout gridLayout, int startPoint, int i, int j)
        {
            // get frame with index startPoint from array of MatrixGrid
            var child = (TextView)gridLayout.GetChildAt(startPoint);

            // assign value of entered Entry
            try
            {
                matrix[i, j] = double.Parse(child.Text);
            }
            catch (Exception e)
            {
                Error.Message = $"Couldn't parse value of {nameof(child)} {{\"{child.Text}\"}} at [{j};{i}].";
                Error.InnerMessage = $"Wrong values\n{gridLayout.GetMatrixValue()}";
                return null;
            }

            return matrix;
        }
    }
}