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
                new double[] { },
                new double[] { }
            };

            var convertedMatrix = Matrix<double>.Build.DenseOfColumns(matrix);

            var n = new int();
            var output = MatrixHighLevel.Exponentiation(convertedMatrix, n);
        }

        private void RenderLayout()
        {
            RenderMatrixFrames();
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
        }

        private void RenderInputFrames(int columns, int rows)
        {
            int count = 1;
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    var entry = new Entry()
                    {
                        Keyboard = Keyboard.Numeric,
                        
                    };
                    MatrixGrid.RowDefinitions[i].Height = 70;
                    MatrixGrid.ColumnDefinitions[i].Width = 70;
                    MatrixGrid.Children.Add(entry);
                    
                }
            }
        }
    }
}