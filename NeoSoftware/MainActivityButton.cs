using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
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
        private EditText _rowsCount;
        private EditText _columnsCount;
        private TextView _resultOutput;
        private TextView _resultInput;
        private TextView _resultTitle;
        private Button _submitSize;
        private Matrix<double> _matrix;
        private Matrix<double> _inputMatrix;
        private Switch _isEquationsSwitch;
        private readonly GridSize _gridSize = new GridSize();


        private void ConfigureRenderingInputFields()
        {
            if (_isEquations)
            {
                ConfigureIsEquationsSwitch();
                ConfigureEquationsGrid();
            }
            else
            {
                ConfigureRowsColumnsCount();
                ConfigureGrid();
            }
        }

        private void GetButtons()
        {
            _transpose = FindViewById<Button>(Resource.Id.transpose_btn);
            _reverse = FindViewById<Button>(Resource.Id.reverse_btn);
            _det = FindViewById<Button>(Resource.Id.det_btn);
            _rank = FindViewById<Button>(Resource.Id.rank_btn);
            _exponentiation = FindViewById<Button>(Resource.Id.exp_btn);

            ConfigureButtons();
        }

        private void ConfigureEquationsGrid()
        {
            _gridLayoutMatrix = FindViewById<GridLayout>(Resource.Id.matrix_grid);
            _gridLayoutMatrix.RowCount = _gridSize.RowCount = int.Parse(_rowsCount.Text);
            _gridLayoutMatrix.ColumnCount = 1;
            RenderGridLayout(childWidth: 900);
        }

        private void ConfigureIsEquationsSwitch()
        {
            _isEquationsSwitch = FindViewById<Switch>(Resource.Id.is_equations_switch);
            _isEquationsSwitch.CheckedChange += delegate { _isEquations = !_isEquations; };
            _isEquationsSwitch.Checked = _isEquations;
        }

        private void ConfigureButtons()
        {
            _transpose.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_inputMatrix, _matrix.Transpose(), ResultKind.Transpose);
            };
            _reverse.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_inputMatrix, MatrixHighLevel.GetReverseMatrix(_matrix), ResultKind.Reverse);
            };
            _det.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_inputMatrix), MatrixHighLevel.GetDeterminant(_matrix).ToString(),
                    ResultKind.Determinant);
            };
            _rank.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_inputMatrix), MatrixHighLevel.GetRank(_matrix).ToString(), ResultKind.Rank);
            };
            _exponentiation.Click += (sender, args) =>
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_matrix,
                    MatrixHighLevel.Exponentiation(_inputMatrix,
                        int.Parse(FindViewById<EditText>(Resource.Id.exp_value).Text)),
                    ResultKind.Exponentiation);
            };
        }

        private void ConfigureRowsColumnsCount()
        {
            _rowsCount = FindViewById<EditText>(Resource.Id.rows_count);
            _columnsCount = FindViewById<EditText>(Resource.Id.columns_count);
            _submitSize = FindViewById<Button>(Resource.Id.submit_size);

            _rowsCount.Text = _gridSize.RowCount.ToString();
            _columnsCount.Text = _gridSize.ColumnCount.ToString();


            if (_rowsCount.Text == string.Empty)
                _rowsCount.Text = "3";
            if (_columnsCount.Text == string.Empty)
                _columnsCount.Text = "3";

            _submitSize.Click += (sender, args) => { ConfigureGrid(); };
        }

        private bool ValidRowsColumnsCount(int rows, int columns)
        {
            return rows <= 6 && rows > 1 && columns <= 6 && columns > 1;
        }

        private void ConfigureGrid()
        {
            if (!ValidRowsColumnsCount(int.Parse(_rowsCount.Text), int.Parse(_columnsCount.Text)))
                return;

            _gridLayoutMatrix = FindViewById<GridLayout>(Resource.Id.matrix_grid);
            _gridLayoutMatrix.RemoveAllViewsInLayout();
            _gridLayoutMatrix.RowCount = _gridSize.RowCount = int.Parse(_rowsCount.Text);
            _gridLayoutMatrix.ColumnCount = _gridSize.ColumnCount = int.Parse(_columnsCount.Text);

            if (HandleMatrixAndroid.GetGridLayout(_inputMatrix, _gridLayoutMatrix, this) is null)
                RenderGridLayout();
        }

        private void RenderGridLayout(int childHeight = 100, int childWidth = 150)
        {
            for (var i = 0; i < _gridLayoutMatrix.RowCount; i++)
            {
                for (var j = 0; j < _gridLayoutMatrix.ColumnCount; j++)
                {
                    var child = new EditText(this)
                    {
                        TextSize = 18,
                        TextAlignment = TextAlignment.Center,
                    };
                    child.SetHeight(childHeight);
                    child.SetWidth(childWidth);
                    _gridLayoutMatrix.AddView(child, i + j);
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
            _isLoadMain = true;
            SetContentView(Resource.Layout.activity_main);
            BuildUI();
        }

        [Export("BackToManual")]
        public void BackToManualBtn(View view)
        {
            _isLoadMain = false;
            SetContentView(Resource.Layout.activity_manual);
            BuildUI();
        }

        private void SetMatrix(GridLayout gridLayout)
        {
            _matrix = HandleMatrixAndroid.GetMatrix(gridLayout);
            _inputMatrix = HandleMatrixAndroid.GetMatrix(gridLayout);
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
                    if (j == matrix.ColumnCount)
                    {
                        message = message.Append("\n");
                        continue;
                    }

                    message = message.Append($"{matrix[j, i]}\t\t");
                }
            }

            return message.ToString();
        }
    }
}