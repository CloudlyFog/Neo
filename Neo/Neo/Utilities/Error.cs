namespace Neo.Utilities;

public class Error
{
    public Error(string message = "No error")
    {
        Message = message;
    }

    public static string Message { get; set; } = "No error";
    public static string InnerMessage { get; set; }
    public static string ArgValues { get; set; }
}