using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            SetMatrixFrames();
        }


        private void Exp_OnClick(object sender, EventArgs e)
        {
            var matrix = new[]
            {
                new double[] { },
                new double[] { }
            };

            var convertedMatrix = Matrix<double>.Build.DenseOfColumns(matrix);

            var n = new int();
            var output = MatrixHighLevel.Exponentiation(convertedMatrix, n);
        }

        private void SetMatrixFrames()
        {
            MatrixFrame.BackgroundColor = Color.White;
            MatrixFrame.CornerRadius = 21;
            MatrixFrame.HeightRequest = 250;
            MatrixFrame.WidthRequest = 100;
            MatrixFrame.Margin = 
                new Thickness(33, 105, 38, 0);

            InputFrame.Background = Brush.White;
            InputFrame.CornerRadius = 21;
            InputFrame.WidthRequest = 70;
            InputFrame.HeightRequest = 40;

        }

        private void RenderInputFrames(int columns, int rows)
        {
            for (int j = 0; j < columns; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    
                }
            }
        }
    }
}
