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
        /// <param name="input">parsed data from image by Tesseract OCR </param>
        /// <returns></returns>
        public static Vector<double> SolveLinearEquation(string input)
            => MatrixEquation.GetMatrixEquation(input).LeftSide
                .Solve(MatrixEquation.GetMatrixEquation(input).RightSide);
        
        /// <summary>
        /// read matrix from image by Tesseract OCR
        /// </summary>
        /// <param name="path">path to image</param>
        /// <param name="rowsCount">row's count of matrix</param>
        /// <returns></returns>
        private string Read(string path, int rowsCount)
        {
            var ocr = new IronTesseract();
            var img = Image.FromFile(path);
            ResizeImage(img, 300, 300);

            // idk why x equals 1
            var contentArea = new CropRectangle(x: 1, y: 0, height: img.Height / rowsCount, width: img.Width);
            var input = new OcrInput(path, contentArea);
            var sb = new StringBuilder();
            for (int i = 1; i <= rowsCount; i++)
            {
                if (img.Height / i != img.Height)
                    // crop the image so that only one line is visible
                    contentArea = new CropRectangle(x: 0, y: 0, height: img.Height / rowsCount, width: img.Width);
                input = new OcrInput(path, contentArea);
                input.Deskew();
                input.EnhanceResolution();
                input.DeNoise();
                input.Contrast();
                input.Binarize();
                sb = sb.Append($"{ocr.Read(input).Text}{MatrixEquation.SplitSymbol}");
            }

            img.Dispose();
            input.Dispose();
            return sb.ToString();
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
    }
}