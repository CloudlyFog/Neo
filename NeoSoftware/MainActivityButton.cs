using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Java.Interop;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using NeoSoftware.Services;


namespace NeoSoftware
{
    public partial class MainActivity
    {
        private Button _transpose;
        private Button _reverse;
        private Button _det;
        private Button _rank;
        private Button _exponentiation;
        private GridLayout _gridLayoutMatrix;
        private Matrix<double> _matrix;
        private int _rows;
        private int _columns;

        private void GetButtons()
        {
            _transpose = FindViewById<Button>(Resource.Id.transpose_btn);
            _reverse = FindViewById<Button>(Resource.Id.reverse_btn);
            _det = FindViewById<Button>(Resource.Id.det_btn);
            _rank = FindViewById<Button>(Resource.Id.rank_btn);
            _exponentiation = FindViewById<Button>(Resource.Id.exp_btn);

            ConfigureButtons();
        }

        private void ConfigureButtons()
        {
            _transpose.Click += (s, a) =>
            {
                SetMatrix(_gridLayoutMatrix, _gridLayoutMatrix.RowCount, _gridLayoutMatrix.ColumnCount);
            };
        }

        private void ConfigureGrid()
        {
            _gridLayoutMatrix = FindViewById<GridLayout>(Resource.Id.matrix_grid);
            _gridLayoutMatrix.ColumnCount = 3;
            _gridLayoutMatrix.RowCount = 3;

            for (var i = 0; i < _gridLayoutMatrix.RowCount; i++)
            {
                for (var j = 0; j < _gridLayoutMatrix.ColumnCount; j++)
                {
                    _gridLayoutMatrix.AddView(new TextView(this)
                    {
                    });
                }
            }
        }

        [Export("Solve")]
        public void OpenConfirmationDialog(View view)
        {
            _detect = false;
            _detectSwitch.Checked = _detect;
            _confirmationAlertDialog.Show();
        }

        [Export("Confirm")]
        public void ConfirmDataInput(View view)
        {
            _detectSwitch.Checked = _detect;
            _confirmationAlertDialog.Cancel();
            _output.Text = new Solver(_tessOutput.Text);
        }

        [Export("BackToRecognition")]
        public void BackToRecognitionBtn(View view)
            => SetContentView(Resource.Layout.activity_manual);

        [Export("BackToManual")]
        public void BackToManualBtn(View view) =>
            SetContentView(Resource.Layout.activity_manual);

        [Export("Transpose")]
        public void Transpose(View view)
        {
        }

        [Export("Reverse")]
        public void Reverse(View view)
        {
        }

        [Export("Determinant")]
        public void Determinant(View view)
        {
        }

        [Export("Rank")]
        public void Rank(View view)
        {
        }

        [Export("Exp")]
        public void Exponentiation(View view)
        {
        }

        private void SetMatrix(GridLayout gridLayout, int rows, int columns)
        {
            if (_matrix != null)
                return;
            _matrix = HandleMatrixAndroid.GetMatrix(gridLayout, rows, columns);
        }

        private void ShowResult(Matrix<double> output, ResultKind resultKind)
        {
            var message = new StringBuilder();
            for (var i = 0; i < output.RowCount; i++)
            {
                for (var j = 0; j <= output.ColumnCount; j++)
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


        public async Task DisplayAlert(string title, string message, string cancel)
        {
        }
    }
}