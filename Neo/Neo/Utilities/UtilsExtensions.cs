using System.Collections.Generic;
using System.Text;

namespace Neo.Utilities;

public static class UtilsExtensions
{
    public static string GetArrayValue<T>(this T[,] array)
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

    public static string GetArrayValue<T>(this IEnumerable<T> array)
    {
        var sb = new StringBuilder();
        foreach (var item in array)
            sb.Append(item);

        return sb.ToString();
    }

    /// <summary>
    /// valid passed two dimension array for the some specifications
    /// </summary>
    /// <param name="array">passed array</param>
    /// <param name="arrayName">name of passed array (name of variable)</param>
    /// <typeparam name="T">the type of elements in the array</typeparam>
    /// <returns></returns>
    public static Error ValidArray<T>(this T[,] array, string arrayName)
    {
        if (array is null)
        {
            Error.Message = $"{arrayName} is null.";
            return null;
        }

        if (array.Length <= 0)
        {
            Error.Message = $"length of {arrayName} less or equals 0";
            return null;
        }

        return new Error();
    }

    /// <summary>
    /// valid passed array for the some specifications
    /// </summary>
    /// <param name="array">passed array</param>
    /// <param name="arrayName">name of passed array (name of variable)</param>
    /// <typeparam name="T">the type of elements in the array</typeparam>
    /// <returns></returns>
    public static Error ValidArray<T>(this T[] array, string arrayName)
    {
        if (array is null)
        {
            Error.Message = $"{arrayName} is null.";
            return null;
        }

        if (array.Length <= 0)
        {
            Error.Message = $"length of {arrayName} less or equals 0";
            return null;
        }

        return new Error();
    }
}