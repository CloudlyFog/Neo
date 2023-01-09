namespace Neo.Utilities;

public class Error
{
    public Error(string message)
    {
        Message = message;
    }

    public static string Message { get; set; }
    public static string InnerMessage { get; set; }
    public static string ArgValues { get; set; }
}