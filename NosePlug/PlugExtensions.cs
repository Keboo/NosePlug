namespace NosePlug;

/// <summary>
/// Contains extension methods for returning vlaue rather than needing a delegate.
/// </summary>
public static class PlugExtensions
{
    /// <summary>
    /// Speifies a value that will be returned from the method.
    /// </summary>
    /// <param name="plug">The method plug</param>
    /// <param name="returnValue">The value to return from the method</param>
    /// <returns>The original method plug</returns>
    public static IMethodPlug<TReturn> Returns<TReturn>(this IMethodPlug<TReturn> plug, TReturn returnValue)
    {
        if (plug is null)
        {
            throw new ArgumentNullException(nameof(plug));
        }
        return plug.Returns(() => returnValue);
    }

    /// <summary>
    /// Speifies a value that will be returned from the property's get method.
    /// </summary>
    /// <param name="plug">The property plug</param>
    /// <param name="returnValue">The value to return from the property</param>
    /// <returns>The original property plug</returns>
    public static IPropertyPlug<TProperty> Returns<TProperty>(this IPropertyPlug<TProperty> plug, TProperty returnValue)
    {
        if (plug is null)
        {
            throw new ArgumentNullException(nameof(plug));
        }
        return plug.Returns(() => returnValue);
    }
}
