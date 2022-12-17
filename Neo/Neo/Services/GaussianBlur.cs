using System;
using System.Drawing;

namespace Neo.Services
{
    internal sealed class GaussianBlur
    {
        /// <summary>
        /// blur image by Gaussian algorithm
        /// </summary>
        /// <param name="image">image for blurring</param>
        /// <param name="deviation">value of blurring. 3 is optimal value for blurring</param>
        /// <returns></returns>
        public static Bitmap FilterProcessImage(Bitmap image, double deviation = 1)
        {
            var ret = new Bitmap(image.Width, image.Height);
            var matrix = new double[image.Width, image.Height];
            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                    matrix[i, j] = Grayscale(image.GetPixel(i, j)).R;
            }

            matrix = GaussianConvolution(matrix, deviation);
            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    var val = (int)Math.Min(255, matrix[i, j]);
                    ret.SetPixel(i, j, Color.FromArgb(255, val, val, val));
                }
            }

            return ret;
        }

        private static double[,] Calculate1DSampleKernel(double deviation, int size)
        {
            var ret = new double[size, 1];
            double sum = 0;
            var half = size / 2;
            for (var i = 0; i < size; i++)
            {
                ret[i, 0] = 1 / (Math.Sqrt(2 * Math.PI) * deviation) *
                            Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation));
                sum += ret[i, 0];
            }

            return ret;
        }

        private static double[,] Calculate1DSampleKernel(double deviation)
        {
            var size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
            return Calculate1DSampleKernel(deviation, size);
        }

        private static double[,] CalculateNormalized1DSampleKernel(double deviation)
        {
            return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        }

        private static double[,] GaussianConvolution(double[,] matrix, double deviation)
        {
            var kernel = CalculateNormalized1DSampleKernel(deviation);
            var res1 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            var res2 = new double[matrix.GetLength(0), matrix.GetLength(1)];
            //x-direction
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                    res1[i, j] = ProcessPoint(matrix, i, j, kernel, 0);
            }

            //y-direction
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                    res2[i, j] = ProcessPoint(res1, i, j, kernel, 1);
            }

            return res2;
        }

        private static double ProcessPoint(double[,] matrix, int x, int y, double[,] kernel, int direction)
        {
            double res = 0;
            var half = kernel.GetLength(0) / 2;
            for (var i = 0; i < kernel.GetLength(0); i++)
            {
                var cox = direction == 0 ? x + i - half : x;
                var coy = direction == 1 ? y + i - half : y;
                if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
                {
                    res += matrix[cox, coy] * kernel[i, 0];
                }
            }

            return res;
        }

        private static Color Grayscale(Color cr)
        {
            return Color.FromArgb(cr.A, (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
                (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11),
                (int)(cr.R * .3 + cr.G * .59 + cr.B * 0.11));
        }

        private static double[,] NormalizeMatrix(double[,] matrix)
        {
            var ret = new double[matrix.GetLength(0), matrix.GetLength(1)];
            double sum = 0;
            for (var i = 0; i < ret.GetLength(0); i++)
            {
                for (var j = 0; j < ret.GetLength(1); j++)
                    sum += matrix[i, j];
            }

            if (sum != 0)
            {
                for (var i = 0; i < ret.GetLength(0); i++)
                {
                    for (var j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = matrix[i, j] / sum;
                }
            }

            return ret;
        }
    }
}