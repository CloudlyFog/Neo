using System.Text;

namespace Neo.Utilities;

public static class UtilsExtensions
{
    public static string GetArray(this double[,] array)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < array.GetLength(0); i++)
        {
            for (var j = 0; j < array.GetLength(1); j++)
                sb.Append(array[i, j]);

            sb.Append('\n');
        }

        return sb.ToString();
    }
}