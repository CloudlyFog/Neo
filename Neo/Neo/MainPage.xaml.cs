using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using Xamarin.Forms;

namespace Neo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            RenderLayout();
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

        private double[,] AddingDataToMatrix()
        {
            var matrix = new double[3,3];
            var startPoint = 0;
            var j = 0;
            for (int i = 0; startPoint < MatrixGrid.Children.Count; i++)
            {
                ValidateIterators(ref i, ref j);
                matrix = SettingValue(matrix, startPoint, i, j);
                startPoint++;
            }

            return matrix;
        }

        private void ValidateIterators(ref int i, ref int j)
        {
            if (i == 3)
            {
                i = 0;
                j++;
            }
            if (j == 3) j = 0;
        }

        private double[,] SettingValue(double[,] matrix, int startPoint, int i, int j)
        {
            var frame = (Frame)MatrixGrid.Children[startPoint];
            var entry = (Entry)frame.Content;
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
                    if (j == 3)
                    {
                        message = message.Append("\n");
                        continue;
                    }
                    message = message.Append($"{output[i, j]}\t");
                }
            }
            DisplayAlert(resultKind.ToString(), message.ToString(), "Close");
        }
        
        private void ShowResult(string output, ResultKind resultKind)
            => DisplayAlert(resultKind.ToString(), output, "Close");

        private void RenderLayout()
        {
            RenderMatrixFrames();
            RenderInputFrames(3, 3);
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
                        BorderColor = Color.Orchid
                    };
                    MatrixGrid.Children.Add(frame, j, i);
                }

            }
        }
    }
}