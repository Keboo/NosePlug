namespace NosePlug;

/// <summary>
/// A base exception type that is thrown
/// </summary>
public class NasalException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NasalException"/>
    /// </summary>
    public NasalException()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NasalException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message</param>
    public NasalException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NasalException"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public NasalException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
