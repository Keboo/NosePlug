using System;

namespace NosePlug;

/// <summary>
/// Contains extension methods for returning vlaue rather than needing a delegate.
/// </summary>
public static class PlugExtensions
{
    public static IMethodPlug<TReturn> Returns<TReturn>(this IMethodPlug<TReturn> plug, TReturn returnValue)
    {
        if (plug is null)
        {
            throw new ArgumentNullException(nameof(plug));
        }
        return plug.Returns(() => returnValue);
    }

    public static IPropertyPlug<TProperty> Returns<TProperty>(this IPropertyPlug<TProperty> plug, TProperty returnValue)
    {
        if (plug is null)
        {
            throw new ArgumentNullException(nameof(plug));
        }
        return plug.Returns(() => returnValue);
    }
}
