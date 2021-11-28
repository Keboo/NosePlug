using System;
using System.Runtime.Serialization;

namespace NosePlug;

public class NasalException : Exception
{
    public NasalException()
    {
    }

    public NasalException(string? message)
        : base(message)
    {
    }

    public NasalException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected NasalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
