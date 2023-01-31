using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Java.Interop;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using NeoSoftware.Services;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;


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
        private TextView _resultOutput;
        private View _resultWindow;
        private AlertDialog _resultAlertDialog;
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
            _transpose.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(MatrixHighLevel.Transpose(_matrix), ResultKind.Transpose);
            };
            _reverse.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(MatrixHighLevel.GetReverseMatrix(_matrix), ResultKind.Reverse);
            };
            _det.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(MatrixHighLevel.GetDeterminant(_matrix).ToString(), ResultKind.Determinant);
            };
            _rank.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(MatrixHighLevel.GetRank(_matrix).ToString(), ResultKind.Rank);
            };
            _exponentiation.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(
                    MatrixHighLevel.Exponentiation(_matrix,
                        int.Parse(FindViewById<EditText>(Resource.Id.exp_value).Text)),
                    ResultKind.Exponentiation);
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
                    _gridLayoutMatrix.AddView(new EditText(this), i + j);
                }
            }
        }

        private void ConfigureResultWindow()
        {
            _resultOutput = FindViewById<TextView>(Resource.Id.result_output);
            _resultWindow = LayoutInflater.Inflate(Resource.Layout.result, null);
            _resultAlertDialog = new AlertDialog.Builder(this).Create();
            _resultAlertDialog.SetCancelable(true);
            _resultAlertDialog.SetView(_resultOutput);
        }

        [Export("Solve")]
        public void OpenConfirmationDialog(View view)
        {
            _detectSwitch.Checked = _detect = false;
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
        {
            isLoadMain = true;
            SetContentView(Resource.Layout.activity_main);
            BuildUI();
        }

        [Export("BackToManual")]
        public void BackToManualBtn(View view)
        {
            isLoadMain = false;
            SetContentView(Resource.Layout.activity_manual);
            BuildUI();
        }

        private void SetMatrix(GridLayout gridLayout)
        {
            if (_matrix != null)
                return;
            _matrix = HandleMatrixAndroid.GetMatrix(gridLayout);
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