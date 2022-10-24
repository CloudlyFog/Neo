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
            var matrix = new double[3,3];
            var frame = MatrixGrid.Children[0] as Frame;
            var entry = frame.Content as Entry;
            // нужно обратиться потом к свойству Content, котоырй будет содержать объект Entry
            // и затем оттуда достать значение через Text
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[j, i] = double.Parse(entry.Text);
                }
            }

            var convertedMatrix = Matrix<double>.Build.DenseOfArray(matrix);
            
            
            var n = 3;
            var output = MatrixHighLevel.Exponentiation(convertedMatrix, n);
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