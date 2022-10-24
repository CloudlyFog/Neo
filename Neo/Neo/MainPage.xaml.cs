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
            var matrix = new[]
            {
                new double[] { }
                // new double[]
                // {
                //     double.Parse(Value1.Text), 
                //     double.Parse(Value2.Text), 
                //     double.Parse(Value3.Text)
                // },
                // new double[]
                // {
                //     double.Parse(Value4.Text), 
                //     double.Parse(Value5.Text), 
                //     double.Parse(Value6.Text)
                // },
                // new double[]
                // {
                //     double.Parse(Value7.Text), 
                //     double.Parse(Value8.Text), 
                //     double.Parse(Value9.Text)
                // }
            };

            var convertedMatrix = Matrix<double>.Build.DenseOfColumns(matrix);

            var n = 3;
            var output = MatrixHighLevel.Exponentiation(convertedMatrix, n);
            Console.WriteLine(output);
        }

        private void RenderLayout()
        {
            RenderMatrixFrames();
            RenderButtonFrames();
            RenderInputFrames(3, 3);
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
                    MatrixGrid.ColumnDefinitions[i] = new ColumnDefinition()
                    {
                        Width = 70
                    };
                    MatrixGrid.Children.Add(frame, i, j);
                }

                MatrixGrid.RowDefinitions[j] = new RowDefinition();
            }
        }
    }
}