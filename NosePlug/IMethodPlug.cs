using System;

namespace NosePlug;

/// <summary>
/// The base interface for plugs
/// </summary>

public interface IPlug
{

}


/// <summary>
/// A method plug for a void returning method.
/// </summary>
public partial interface IMethodPlug : IPlug
{
    /// <summary>
    /// Indicates if the original plugged static method should be called.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IMethodPlug CallOriginal(bool shouldCallOriginal = true);
}

/// <summary>
/// A method plug for a method with a return value.
/// </summary>
/// <typeparam name="TReturn"></typeparam>
public partial interface IMethodPlug<TReturn> : IPlug
{
    /// <summary>
    /// Indicates if the original plugged static method should be called.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}
