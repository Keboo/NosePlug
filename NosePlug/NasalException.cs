using System.Runtime.Serialization;

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

    /// <summary>
    /// Initializes a new instance of the System.Exception class with serialized data.
    /// </summary>
    /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    protected NasalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
