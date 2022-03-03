namespace NosePlug;

/// <summary>
/// The base interface for plugs
/// </summary>

public interface IPlug
{
}


/// <summary>
/// A method plug for a void returning static method.
/// </summary>
public partial interface IMethodPlug : IPlug
{
    /// <summary>
    /// Indicates if the original plugged static method should be called.
    /// If any callback is registered it will be invoked before calling the original method.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IMethodPlug CallOriginal(bool shouldCallOriginal = true);
}

/// <summary>
/// A method plug for a static method with a return value.
/// </summary>
/// <typeparam name="TReturn">The type of return value from the method</typeparam>
public partial interface IMethodPlug<TReturn> : IPlug
{
    /// <summary>
    /// Indicates if the original plugged static method should be called.
    /// If any callback is registered it will be invoked before calling the original method.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}

/// <summary>
/// A method plug for a void returning instance method.
/// </summary>
public partial interface IInstanceMethodPlug : IPlug
{
    /// <summary>
    /// Indicates if the original plugged instance method should be called.
    /// If any callback is registered it will be invoked before calling the original method.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IInstanceMethodPlug CallOriginal(bool shouldCallOriginal = true);
}

/// <summary>
/// A method plug for a instance method with a return value.
/// </summary>
/// <typeparam name="TReturn">The type of return value from the method</typeparam>
public partial interface IInstanceMethodPlug<TReturn> : IPlug
{
    /// <summary>
    /// Indicates if the original plugged instance method should be called.
    /// If any callback is registered it will be invoked before calling the original method.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IInstanceMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}