using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// <param name="leftMatrix">left side of matrix </param>
        /// <param name="rightMatrix">digits after |'s symbol (equals) in matrix. In other words result of equation</param>
        /// <returns></returns>
        public static double[] SolveLinearEquation(Matrix<double> leftMatrix, Vector<double> rightMatrix) 
            => leftMatrix.Solve(rightMatrix).Select(x => Math.Round(x)).ToArray();
        
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
                sb = sb.Append($"{ocr.Read(input)},");
            }
            img.Dispose();
            input.Dispose();
            return Parse(sb.ToString());
        }

        private Matrix<double> Parse(string input)
        {
            var sourceArray = new double[
                // set count of columns by parsing input text
                // at the end of any matrix's row will types comma
                input.Count(x => x == ',') + 1,
                
                // takes all split elements before comma
                input.TakeWhile(x => x != ',').Count(x => !char.IsWhiteSpace(x)) - 1];
            
            
            // removing white space and commas
            var filterResult = input.Split(' ', ',').Where(x => x != " " || x != "").ToList();
            
            // removing empty space
            filterResult.Remove("");
            ConvertTo(sourceArray, filterResult);

            return Matrix<double>.Build.DenseOfArray(sourceArray);
        }

        /// <summary>
        /// Adding data to <see cref="sourceArray" />
        /// </summary>
        /// <param name="sourceArray">array which will contain parsed data from filterResult</param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        private void ConvertTo(double[,] sourceArray, List<string> filterResult)
        {
            // start point for input text
            // like an iterator
            var point = 0;
            
            for (int i = 0; i < sourceArray.GetLength(0);)
            {
                // on every iteration we increasing point instead of j
                // because we have to iterate filterResult but not columns of sourceArray
                for (int j = 0; j < sourceArray.GetLength(1); point++)
                {
                    if (!Validate(ref point, filterResult))
                        break;
                    sourceArray[i, j] = double.Parse(filterResult[point]);
                    j++;
                }
                i++;
            }
        }
        
        /// <summary>
        /// Validate index of filterResult is corresponding to requirements or not
        /// </summary>
        /// <param name="point">index of <see cref="filterResult"/></param>
        /// <param name="filterResult">parsed data from Tesseract OCR</param>
        /// <returns></returns>
        private bool Validate(ref int point, List<string> filterResult)
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