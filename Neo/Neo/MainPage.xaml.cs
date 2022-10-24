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
        {

            var matrix = AddingDataToMatrix();
            var convertedMatrix = Matrix<double>.Build.DenseOfArray(matrix);
            ShowResult(MatrixHighLevel.Exponentiation(convertedMatrix, int.Parse(ExpValue.Text)));
        }

        private double[,] AddingDataToMatrix()
        {
            var matrix = new double[3,3];
            var startPoint = 0;
            var j = 0;
            for (int i = 0; startPoint < MatrixGrid.Children.Count; i++)
            {
                if (i == 3)
                {
                    i = 0;
                    j++;
                }
                var frame = (Frame)MatrixGrid.Children[startPoint];
                var entry = (Entry)frame.Content;
                matrix[i, j] = double.Parse(entry.Text);
                if (j == 3) j = 0;

                startPoint++;
            }

            return matrix;
        }

        private void ShowResult(Matrix<double> output)
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
            DisplayAlert("Exponentiation", message.ToString(), "Close");
        }

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
            Exp.CornerRadius = 16;
            Exp.BackgroundColor = Color.White;
            Exp.Opacity = 40;

            Transpose = Exp;
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