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
    /// If any callback is registered it will be invoked before calling the original method.
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
    /// If any callback is registered it will be invoked before calling the original method.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original method should be called. Defaults <c>true</c></param>
    /// <returns>The original method plug instance</returns>
    IMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}

public partial interface IInstanceMethodPlug : IPlug
{
    IInstanceMethodPlug CallOriginal(bool shouldCallOriginal = true);
}

public partial interface IInstanceMethodPlug<TReturn> : IPlug
{
    IInstanceMethodPlug<TReturn> CallOriginal(bool shouldCallOriginal = true);
}