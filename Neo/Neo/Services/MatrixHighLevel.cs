using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Neo.Services
{
    public class MatrixHighLevel
    {

        /// <summary>
        /// returns determinant of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetDeterminant(Matrix<double> matrix) 
            => matrix.Determinant();

        /// <summary>
        /// returns rank of matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetRank(Matrix<double> matrix)
            => matrix.Rank();

        /// <summary>
        /// reversing matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> GetReverseMatrix(Matrix<double> matrix)
            => matrix.Inverse();
        
        /// <summary>
        /// Raises the matrix to the specified power
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="n">Exponentiation value</param>
        /// <returns></returns>
        public static Matrix<double> Exponentiation(Matrix<double> matrix, int n)
        {
            var tempMatrix = matrix;
            for (int i = 1; i < n; i++)
                matrix = matrix.Multiply(tempMatrix);
            return matrix;
        }

        /// <summary>
        /// transposing matrix
        /// column go to row and row go to column
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<double> Transpose(Matrix<double> matrix)
            => matrix.Transpose();

        /// <summary>
        /// solving linear equation
        /// </summary>
        /// <param name="leftSideMatrix">left side of matrix </param>
        /// <param name="rightSideMatrix">digits after |'s symbol (equals) in matrix. In other words result of equation</param>
        /// <returns></returns>
        public static double[] SolveLinearEquation(string leftSideMatrix, string rightSideMatrix) 
            => ParseToMatrix(leftSideMatrix)
                .Solve(ParseToVector(rightSideMatrix))
                .Select(x => Math.Round(x)).ToArray();
        
        private Matrix<double> Read(string path, int rowsCount)
        {
            var ocr = new IronTesseract();
            var img = Image.FromFile(path);

            var contentArea = new CropRectangle(x: 1, y: 0, height: img.Height / rowsCount, width: img.Width);
            var input = new OcrInput(path, contentArea);
            var sb = new StringBuilder();
            for (int i = 1; i <= rowsCount; i++)
            {
                if (img.Height / i != img.Height)
                    contentArea = new CropRectangle(x: 0, y: 0, height: img.Height / rowsCount, width: img.Width);
                input = new OcrInput(path, contentArea);
                ResizeImage(img, 300, 300);
                input.Deskew();
                input.EnhanceResolution();
                input.DeNoise();
                input.Contrast();
                input.Binarize();
                sb = sb.Append($"{ocr.Read(input).Text},");
            }

            img.Dispose();
            input.Dispose();
            return ParseToMatrix(sb.ToString());
        }
        
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static void ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }
        
        /// <summary>
        /// Parse data from <see cref="input"/> to <see cref="Matrix{T}"/>
        /// </summary>
        /// <param name="input">output of Tesseract OCR</param>
        /// <returns></returns>
        private static Matrix<double> ParseToMatrix(string input)
        {
            var targetArray = new double[
                // set count of columns by parsing input text
                // at the end of any matrix's row will types comma
                input.Count(x => x == ',') + 1,
                
                // takes all elements before comma
                input.TakeWhile(x => x != ',').Count(x => !char.IsWhiteSpace(x)) - 1];
            
            
            // removing white space and commas
            var filterResult = input.Split(' ', ',').Where(x => x != " " || x != string.Empty).ToList();
            
            // removing empty space
            filterResult.Remove("");
            AddToMatrix(targetArray, filterResult);

            return Matrix<double>.Build.DenseOfArray(targetArray);
        }
        
        /// <summary>
        /// Parse data from <see cref="input"/> to <see cref="Matrix{T}"/>
        /// </summary>
        /// <param name="input">output of Tesseract OCR</param>
        /// <returns></returns>
        private static Vector<double> ParseToVector(string input)
        {
            var targetArray = new double[
                // set count of columns by parsing input text
                // at the end of any matrix's row will types comma
                input.Count(x => x == ',') + 1];
            
            
            // removing white space and commas
            var filterResult = input.Split(' ', ',').Where(x => x != " " || x != string.Empty).ToList();
            
            // removing empty space
            filterResult.Remove("");
            AddToVector(targetArray, filterResult);

            return Vector<double>.Build.DenseOfArray(targetArray);
        }


        /// <summary>
        /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
        /// </summary>
        /// <param name="targetArray">array which will contain parsed data from <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        private static void AddToMatrix(double[,] targetArray, List<string> filterResult)
        {
            // start point for input text
            // like an iterator
            var point = 0;
            
            for (int i = 0; i < targetArray.GetLength(0);)
            {
                // on every iteration we increasing point instead of j
                // because we have to iterate filterResult but not columns of sourceArray
                for (int j = 0; j < targetArray.GetLength(1); point++)
                {
                    if (!Validate(ref point, filterResult))
                        break;
                    targetArray[i, j] = double.Parse(filterResult[point]);
                    j++;
                }
                i++;
            }
        }
        
        
        /// <summary>
        /// Adding data to <see cref="targetArray"/> from <see cref="filterResult"/>
        /// </summary>
        /// <param name="targetArray">array which will contain parsed data from <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        private static void AddToVector(double[] targetArray, List<string> filterResult)
        {
            // start point for input text
            // like an iterator
            var point = 0;
            
            for (int i = 0; i < targetArray.GetLength(0);)
            {
                // on every iteration we increasing point instead of j
                // because we have to iterate filterResult but not columns of sourceArray
                if (!Validate(ref point, filterResult))
                    break;
                targetArray[i] = double.Parse(filterResult[point]);
                i++;
            }
        }
        
        /// <summary>
        /// Validate index of filterResult is corresponding to requirements or not
        /// </summary>
        /// <param name="point">index of <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        /// <returns></returns>
        private static bool Validate(ref int point, List<string> filterResult)
        {
            if (point == filterResult.Count)
                return false;
            if (filterResult[point] == string.Empty)
            {
                point++;
                return false;
            }
            return true;
        }
    }
}