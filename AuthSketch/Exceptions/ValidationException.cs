namespace AuthSketch.Exceptions;

public class ValidationException : AuthSketchException
{
    public ValidationException(string message) : base(message)
    {
    }
}
