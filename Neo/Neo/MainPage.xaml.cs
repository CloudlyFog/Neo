using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServices.Services;
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
            var matrix = new double[][]
            {
                new double[] { }
            };
            var n = new int();
            var output = MatrixHighLevel.Exponentiation(matrix, n);
        }

        private void SetMatrixFrames()
        {
            MatrixFrame.BackgroundColor = Color.White;
            MatrixFrame.CornerRadius = 21;
            MatrixFrame.HeightRequest = 250;
            MatrixFrame.WidthRequest = 100;
            MatrixFrame.Margin = 
                new Thickness(33, 105, 38, 0);
        }
    }
}
