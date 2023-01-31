using System.Text;
using System.Threading.Tasks;
using Android.OS;
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
        private TextView _resultInput;
        private TextView _resultTitle;
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

                ShowResult(_matrix, MatrixHighLevel.Transpose(_matrix), ResultKind.Transpose);
            };
            _reverse.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_matrix, MatrixHighLevel.GetReverseMatrix(_matrix), ResultKind.Reverse);
            };
            _det.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_matrix), MatrixHighLevel.GetDeterminant(_matrix).ToString(),
                    ResultKind.Determinant);
            };
            _rank.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_matrix), MatrixHighLevel.GetRank(_matrix).ToString(), ResultKind.Rank);
            };
            _exponentiation.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_matrix,
                    MatrixHighLevel.Exponentiation(_matrix,
                        int.Parse(FindViewById<EditText>(Resource.Id.exp_value).Text)),
                    ResultKind.Exponentiation);
            };
        }

        private void ConfigureGrid()
        {
            _gridLayoutMatrix = FindViewById<GridLayout>(Resource.Id.matrix_grid);
            _gridLayoutMatrix.ColumnCount = _rows = 3;
            _gridLayoutMatrix.RowCount = _columns = 3;

            for (var i = 0; i < _gridLayoutMatrix.RowCount; i++)
            {
                for (var j = 0; j < _gridLayoutMatrix.ColumnCount; j++)
                {
                    _gridLayoutMatrix.AddView(new EditText(this), i + j);
                }
            }
        }

        private void OpenResultWindow(string title, string input, string output)
        {
            SetContentView(Resource.Layout.result);
            _resultTitle = FindViewById<TextView>(Resource.Id.result_title);
            _resultInput = FindViewById<TextView>(Resource.Id.result_input);
            _resultOutput = FindViewById<TextView>(Resource.Id.result_output);

            _resultTitle.Text = title;
            _resultInput.Text += $"\n{input}";
            _resultOutput.Text += $"\n{output}";
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

        private void ShowResult(Matrix<double> input, Matrix<double> output, ResultKind resultKind)
            => OpenResultWindow(resultKind.ToString(), GetMatrixValue(input), GetMatrixValue(output));

        private void ShowResult(string input, string output, ResultKind resultKind)
            => OpenResultWindow(resultKind.ToString(), input, output);

        private string GetMatrixValue(Matrix<double> matrix)
        {
            var message = new StringBuilder();
            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j <= matrix.ColumnCount; j++)
                {
                    if (j == _columns)
                    {
                        message = message.Append("\n");
                        continue;
                    }

                    message = message.Append($"{matrix[i, j]}\t\t");
                }
            }

            return message.ToString();
        }
    }
}