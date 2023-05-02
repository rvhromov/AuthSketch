namespace AuthSketch.Exceptions;

public class NotFoundException : AuthSketchException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
