using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using System;
using System.Text;
using Xamarin.Forms;

namespace Neo
{
    public partial class MainPage : ContentPage
    {
        private int _columns;
        private int _rows;
        public MainPage()
        {
            InitializeComponent();
            RenderLayout();
            _columns = 1;
            _rows = 1;
        }

        private void Exp_OnClick(object sender, EventArgs e)
            => ShowResult(MatrixHighLevel.Exponentiation(ConvertMatrix(), int.Parse(ExpValue.Text)), ResultKind.Exponentiation);

        private void Transpose_OnClick(object sender, EventArgs e)
            => ShowResult(MatrixHighLevel.Transpose(ConvertMatrix()), ResultKind.Transpose);

        private void Reverse_OnClick(object sender, EventArgs e)
            => ShowResult(MatrixHighLevel.GetReverseMatrix(ConvertMatrix()), ResultKind.Reverse);

        private void Rank_OnClick(object sender, EventArgs e)
            => ShowResult(MatrixHighLevel.GetRank(ConvertMatrix()).ToString(), ResultKind.Rank);

        private void Determinant_OnClick(object sender, EventArgs e)
            => ShowResult(MatrixHighLevel.GetDeterminant(ConvertMatrix()).ToString(), ResultKind.Determinant);

        private Matrix<double> ConvertMatrix()
            => Matrix<double>.Build.DenseOfArray(AddingDataToMatrix());

        /// <summary>
        /// converting data from IGridListView to double[,]
        /// </summary>
        /// <returns></returns>
        private double[,] AddingDataToMatrix()
        {
            var matrix = new double[_columns, _rows];
            var startPoint = 0; // like an i in default cycle
            var j = 0;
            for (int i = 0; startPoint < MatrixGrid.Children.Count; i++)
            {
                if (ValidateIterators(ref i, ref j))
                    break;
                matrix = SettingValue(matrix, startPoint, i, j);
                startPoint++;
            }

            return matrix;
        }

        private bool ValidateIterators(ref int i, ref int j)
        {
            // if i equals count of columns
            // we go to the next row and resetting to zero i
            if (i == _columns)
            {
                i = 0;
                j++;
            }

            // if j equals count of columns
            // we stopping all
            if (j == _rows)
                j = 0;
            return j == 0;
        }

        private double[,] SettingValue(double[,] matrix, int startPoint, int i, int j)
        {
            // get frame with index startPoint from array of MatrixGrid
            var frame = (Frame)MatrixGrid.Children[startPoint];
            
            var entry = (Entry)frame.Content;
            
            // assign value of entered Entry
            matrix[i, j] = double.Parse(entry.Text);
            return matrix;
        }

        private void ShowResult(Matrix<double> output, ResultKind resultKind)
        {
            var message = new StringBuilder();
            for (int i = 0; i < output.RowCount; i++)
            {
                for (int j = 0; j <= output.ColumnCount; j++)
                {
                    if (j == _rows)
                    {
                        message = message.Append("\n");
                        continue;
                    }
                    message = message.Append($"{output[i, j]}|\t\t");
                }
            }
            DisplayAlert(resultKind.ToString(), message.ToString(), "Close");
        }

        private void ShowResult(string output, ResultKind resultKind)
            => DisplayAlert(resultKind.ToString(), output, "Close");

        private void RenderLayout()
        {
            RenderMatrixFrames();
            RenderInputFrames(_columns, _rows);
            RenderButtonFrames();
        }

        private void RenderMatrixFrames()
        {
            MatrixFrame.BackgroundColor = Color.White;
            MatrixFrame.CornerRadius = 21;
            MatrixFrame.HeightRequest = 250;
            MatrixFrame.WidthRequest = 100;
            MatrixFrame.Margin =
                new Thickness(33, 105, 38, 0);

        }
        private void MyEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            Console.WriteLine(entry.IsFocused ? "change from UI" : "change from code");
        }
        private void SetMatrixSize()
        {
            _columns = int.Parse(ColumnCount.Text);
            _rows = int.Parse(RowCount.Text);
        }

        private void RenderButtonFrames()
        {
            StyleExponentiation();
            StyleTranspose();
            StyleDeterminant();
            StyleRank();
            StyleReverse();
        }

        private void StyleExponentiation()
        {
            Exp.CornerRadius = 16;
            Exp.BackgroundColor = Color.White;
            Exp.Opacity = 40;
        }

        private void StyleTranspose()
        {
            Transpose.CornerRadius = 16;
            Transpose.BackgroundColor = Color.White;
            Transpose.Opacity = 40;
        }

        private void StyleDeterminant()
        {
            Determinant.CornerRadius = 16;
            Determinant.BackgroundColor = Color.White;
            Determinant.Opacity = 40;
        }

        private void StyleRank()
        {
            Rank.CornerRadius = 16;
            Rank.BackgroundColor = Color.White;
            Rank.Opacity = 40;
        }

        private void StyleReverse()
        {
            Reverse.CornerRadius = 16;
            Reverse.BackgroundColor = Color.White;
            Reverse.Opacity = 40;
        }

        private void RenderInputFrames(int columns, int rows)
        {
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    var frame = new Frame
                    {
                        Content = new Entry
                        {
                            Keyboard = Keyboard.Numeric
                        },
                        CornerRadius = 20,
                        BorderColor = new Color(254, 173, 167)
                    };
                    MatrixGrid.Children.Add(frame, j, i);
                }
            }
        }
    }
}