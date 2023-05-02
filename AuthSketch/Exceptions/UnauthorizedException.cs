namespace AuthSketch.Exceptions;

public class UnauthorizedException : AuthSketchException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}