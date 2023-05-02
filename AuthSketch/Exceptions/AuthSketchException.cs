namespace AuthSketch.Exceptions;

public abstract class AuthSketchException : Exception
{
    protected AuthSketchException(string message) : base(message)
    {
    }
}
