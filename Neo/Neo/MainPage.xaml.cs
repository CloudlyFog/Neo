using System;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Neo.Services;
using Tesseract;
using Xamarin.Forms;
using XLabs.Ioc;


namespace Neo;

public partial class MainPage
{
    private readonly ITesseractApi _tesseractApi;
    private int _columns;
    private int _rows;
    private const int TakePhotoSizeBtn = 80;
    private const int InputFrameWidth = 60;
    private const int InputFrameHeight = 40;
    private const int ButtonWidth = 200;
    private const int ButtonHeight = 60;

    public Button TakePhoto { get; set; }

    public MainPage()
    {
        _columns = 3;
        _rows = 3;
        InitializeComponent();
        ConfigureTakePhotoButton();
        RenderLayout();

        // StackLayout
        ConfigureStackLayout();

        // _tesseractApi = Resolver.Resolve<ITesseractApi>();
    }

    private void Exp_OnClick(object sender, EventArgs e)
        => ShowResult(MatrixHighLevel.Exponentiation(ConvertToMatrix(GetMatrix()), int.Parse(ExpValue.Text)),
            ResultKind.Exponentiation);

    private void Transpose_OnClick(object sender, EventArgs e)
        => ShowResult(MatrixHighLevel.Transpose(ConvertToMatrix(GetMatrix())), ResultKind.Transpose);

    private void Reverse_OnClick(object sender, EventArgs e)
        => ShowResult(MatrixHighLevel.GetReverseMatrix(ConvertToMatrix(GetMatrix())), ResultKind.Reverse);

    private void Rank_OnClick(object sender, EventArgs e)
        => ShowResult(MatrixHighLevel.GetRank(ConvertToMatrix(GetMatrix())).ToString(), ResultKind.Rank);

    private void Determinant_OnClick(object sender, EventArgs e)
        => ShowResult(MatrixHighLevel.GetDeterminant(ConvertToMatrix(GetMatrix())).ToString(), ResultKind.Determinant);

    public void UpdateMatrixSize(object sender, EventArgs e)
    {
        ColumnCount.TextChanged += (o, args) => { _columns = int.Parse(args.NewTextValue); };


        RowCount.TextChanged += (o, args) => { _rows = int.Parse(args.NewTextValue); };
    }

    private static Matrix<decimal> ConvertToMatrix(decimal[,] matrix)
        => Matrix<decimal>.Build.DenseOfArray(matrix);

    /// <summary>
    /// converting data from IGridListView to double[,]
    /// </summary>
    /// <returns></returns>
    private decimal[,] GetMatrix()
    {
        var matrix = new decimal[_columns, _rows];
        var startPoint = 0; // like an i in default cycle
        var j = 0;
        for (var i = 0; startPoint < MatrixGrid.Children.Count; i++)
        {
            if (ValidateIterators(ref i, ref j))
                break;
            matrix = SetMatrix(matrix, startPoint, i, j);
            startPoint++;
        }

        return matrix;
    }

    private bool ValidateIterators(ref int i, ref int j)
    {
        // if i equals count of columns
        // we go to the next row and resetting to zero i
        if (i == _columns)
        {
            i = 0;
            j++;
        }

        // if j equals count of columns
        // we stopping all
        if (j == _rows)
            j = 0;
        return j == 0;
    }

    private decimal[,] SetMatrix(decimal[,] matrix, int startPoint, int i, int j)
    {
        // get frame with index startPoint from array of MatrixGrid
        var frame = (Frame)MatrixGrid.Children[startPoint];

        var entry = (Entry)frame.Content;

        // assign value of entered Entry
        matrix[i, j] = decimal.Parse(entry.Text);
        return matrix;
    }

    private void ShowResult(Matrix<decimal> output, ResultKind resultKind)
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

    private void RenderLayout()
    {
        ConfigureMatrixFrame();
        ConfigureButtons();
    }

    private void ConfigureMatrixFrame()
    {
        MatrixFrame.BackgroundColor = Color.White;
        MatrixFrame.CornerRadius = 21;
        MatrixFrame.WidthRequest = CalculateMatrixFrameWidth();
        MatrixFrame.HeightRequest = CalculateMatrixFrameHeight();
        MatrixFrame.HorizontalOptions = LayoutOptions.CenterAndExpand;
        MatrixFrame.VerticalOptions = LayoutOptions.CenterAndExpand;
        ConfigureInputFrames(_columns, _rows);
        ConfigureMatrixGrid();
    }

    private void ConfigureMatrixGrid()
    {
        MatrixGrid.HorizontalOptions = LayoutOptions.CenterAndExpand;
        MatrixGrid.VerticalOptions = LayoutOptions.CenterAndExpand;
    }

    private int CalculateMatrixFrameHeight()
    {
        var height = decimal.Multiply(InputFrameHeight, _rows + 1);
        return Convert.ToInt32(height);
    }

    private int CalculateMatrixFrameWidth()
    {
        var width = decimal.Multiply(InputFrameWidth, _columns + 1);
        return Convert.ToInt32(width);
    }

    private void ConfigureStackLayout()
    {
        var stackLayout = new StackLayout();
        stackLayout.Children.Add(MatrixFrame);
        stackLayout.Children.Add(MatrixConfigureGrid);
        stackLayout.Children.Add(ButtonGrid);


        Content = stackLayout;
    }

    private void ConfigureInputFrames(int columns, int rows)
    {
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                MatrixGrid.Children.Add(new Frame
                {
                    Content = new Entry
                    {
                        Keyboard = Keyboard.Numeric,
                        FontSize = 10,
                    },
                    CornerRadius = 20,
                }, column, row);
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = InputFrameWidth,
                });
            }

            MatrixGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = InputFrameHeight,
            });
        }
    }

    private void ConfigureTakePhotoButton()
    {
        // take photo
        TakePhoto = new Button
        {
            CornerRadius = TakePhotoSizeBtn,
            BorderColor = Color.DimGray,
            BorderWidth = 5,
            BackgroundColor = new Color(252, 186, 3),
            WidthRequest = 60,
            HeightRequest = 60,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand,
        };

        ButtonGrid.Children.Add(TakePhoto, 0, 2);
    }

    private void ConfigureButtons()
    {
        StyleExponentiation();
        StyleTransposeButton();
        StyleDeterminantButton();
        StyleRankButton();
        StyleReverseButton();
    }

    private void StyleExponentiation()
    {
        Exp.WidthRequest = ButtonWidth;
        Exp.HeightRequest = ButtonHeight;
        Exp.CornerRadius = 16;
        Exp.BackgroundColor = Color.White;
        Exp.Opacity = 40;
    }

    private void StyleTransposeButton()
    {
        Transpose.WidthRequest = ButtonWidth;
        Transpose.HeightRequest = ButtonHeight;
        Transpose.CornerRadius = 16;
        Transpose.BackgroundColor = Color.White;
        Transpose.Opacity = 40;
    }

    private void StyleDeterminantButton()
    {
        Determinant.WidthRequest = ButtonWidth;
        Determinant.HeightRequest = ButtonHeight;
        Determinant.CornerRadius = 16;
        Determinant.BackgroundColor = Color.White;
        Determinant.Opacity = 40;
    }

    private void StyleRankButton()
    {
        Rank.WidthRequest = ButtonWidth;
        Rank.HeightRequest = ButtonHeight;
        Rank.CornerRadius = 16;
        Rank.BackgroundColor = Color.White;
        Rank.Opacity = 40;
    }

    private void StyleReverseButton()
    {
        Reverse.WidthRequest = ButtonWidth;
        Reverse.HeightRequest = ButtonHeight;
        Reverse.CornerRadius = 16;
        Reverse.BackgroundColor = Color.White;
        Reverse.Opacity = 40;
    }
}