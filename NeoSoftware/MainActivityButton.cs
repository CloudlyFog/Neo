using System.Text;
using Android.Graphics;
using Android.Widget;
using Java.Interop;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using NeoSoftware.Services;
using Button = Android.Widget.Button;
using Switch = Android.Widget.Switch;
using TextAlignment = Android.Views.TextAlignment;
using View = Android.Views.View;


namespace NeoSoftware
{
    public partial class MainActivity
    {
        private Button _transpose;
        private Button _reverse;
        private Button _det;
        private Button _rank;
        private Button _exponentiation;
        private Button _solveMatrix;
        private GridLayout _gridLayoutMatrix;
        private EditText _rowsCount;
        private EditText _columnsCount;
        private TextView _resultOutput;
        private TextView _resultInput;
        private TextView _resultTitle;
        private Button _submitSize;
        private Matrix<double> _matrix;
        private Matrix<double> _inputMatrix;
        private string _inputEquations;
        private string _equations;
        private Switch _isEquationsSwitch;
        private readonly GridSize _gridSize = new GridSize();


        private void ConfigureRenderingInputFields()
        {
            ConfigureIsEquationsSwitch();
            ConfigureRowsColumnsCount();
            ConfigureGrid();
        }

        private void GetButtons()
        {
            _transpose = FindViewById<Button>(Resource.Id.transpose_btn);
            _reverse = FindViewById<Button>(Resource.Id.reverse_btn);
            _det = FindViewById<Button>(Resource.Id.det_btn);
            _rank = FindViewById<Button>(Resource.Id.rank_btn);
            _exponentiation = FindViewById<Button>(Resource.Id.exp_btn);
            _solveMatrix = FindViewById<Button>(Resource.Id.solve_manual_btn);

            ConfigureButtons();
        }

        private void ConfigureEquationsGrid()
        {
            _gridLayoutMatrix = FindViewById<GridLayout>(Resource.Id.matrix_grid);
            _gridLayoutMatrix.RemoveAllViews();
            _gridLayoutMatrix.RowCount = _gridSize.RowCount = int.Parse(_rowsCount.Text);
            _gridLayoutMatrix.ColumnCount = 1;
            RenderGridLayout(childWidth: 900);
        }

        private void ConfigureIsEquationsSwitch()
        {
            _isEquationsSwitch = FindViewById<Switch>(Resource.Id.is_equations_switch);
            _isEquationsSwitch.CheckedChange += delegate
            {
                _isEquations = _isEquationsSwitch.Checked;
                RenderMainInputElement();
            };
        }


        /// <summary>
        /// renders matrix if <see cref="_isEquations"/> is false and fields for equations if true>
        /// </summary>
        /// <param name="calledRCCConf">if this method is calling from <see cref="ConfigureRowsColumnsCount"/></param>
        private void RenderMainInputElement(bool calledRCCConf = false)
        {
            if (_isEquations)
            {
                ConfigureEquationsGrid();
            }
            else
            {
                if (!calledRCCConf)
                    ConfigureRowsColumnsCount();
                ConfigureGrid();
            }
        }

        private void ConfigureButtons()
        {
            _transpose.Click += delegate
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_inputMatrix, _matrix.Transpose(), ResultKind.Transpose);
            };
            _reverse.Click += delegate
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_inputMatrix, MatrixHighLevel.GetReverseMatrix(_matrix), ResultKind.Reverse);
            };
            _det.Click += delegate
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_inputMatrix), MatrixHighLevel.GetDeterminant(_matrix).ToString(),
                    ResultKind.Determinant);
            };
            _rank.Click += delegate
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(GetMatrixValue(_inputMatrix), MatrixHighLevel.GetRank(_matrix).ToString(), ResultKind.Rank);
            };
            _exponentiation.Click += delegate
            {
                SetMatrix(_gridLayoutMatrix);
                ShowResult(_matrix,
                    MatrixHighLevel.Exponentiation(_inputMatrix,
                        int.Parse(FindViewById<EditText>(Resource.Id.exp_value).Text)),
                    ResultKind.Exponentiation);
            };
            _solveMatrix.Click += delegate
            {
                if (_isEquations)
                {
                    SetEquations(_gridLayoutMatrix);
                    ShowResult(GetEquationsValue(_inputEquations), new Solver(_equations), ResultKind.Solve);
                }
                else
                {
                    SetMatrix(_gridLayoutMatrix);
                    ShowResult(GetMatrixValue(_inputMatrix), new Solver(_matrix), ResultKind.Solve);
                }
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

            _submitSize.Click += (sender, args) => { RenderMainInputElement(true); };
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
                    var child = CreateEditTextInstance(childHeight, childWidth);
                    _gridLayoutMatrix.AddView(child, i + j);
                }
            }
        }

        private EditText CreateEditTextInstance(int childHeight, int childWidth)
        {
            var child = new EditText(this);
            if (_isEquations)
            {
                child = new EditText(this)
                {
                    TextSize = 18,
                    Typeface = Typeface.DefaultBold,
                };
            }
            else
            {
                child = new EditText(this)
                {
                    TextSize = 18,
                    TextAlignment = TextAlignment.Center,
                    Typeface = Typeface.DefaultBold,
                };
            }

            child.SetHeight(childHeight);
            child.SetWidth(childWidth);

            return child;
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

        private void SetEquations(GridLayout gridLayout)
        {
            _equations = GetEquations(gridLayout);
            _inputEquations = GetEquations(gridLayout);
        }

        private static string GetEquationsValue(string equations)
        {
            return equations.Replace(Parser.SplitSymbol, '\n');
        }

        private static string GetEquations(GridLayout gridLayout)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < gridLayout.RowCount; i++)
            {
                var child = (EditText)gridLayout.GetChildAt(i);
                sb.Append($"{child.Text}{Parser.SplitSymbol}");
            }

            return sb.ToString();
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