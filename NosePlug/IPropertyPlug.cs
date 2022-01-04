using System;

namespace NosePlug;

/// <summary>
/// A plug for a property.
/// </summary>
/// <typeparam name="TProperty"></typeparam>
public interface IPropertyPlug<TProperty> : IPlug
{
    /// <summary>
    /// Replace the getter of the property with an alternate delegate that can return a different value.
    /// </summary>
    /// <param name="getReturnValue">The delegate to replace the property's get method</param>
    /// <returns>The original property plug</returns>
    IPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue);
    /// <summary>
    /// Replace the setter of the property with an alternate delegate that can perform alternate logic.
    /// </summary>
    /// <param name="newSetter">The delegate to replace the property's set method</param>
    /// <returns>The original property plug</returns>
    IPropertyPlug<TProperty> Callback(Action<TProperty> newSetter);
    /// <summary>
    /// Indicates if the original plugged property's get/set methods should be called.
    /// </summary>
    /// <param name="shouldCallOriginal">A flag indicating if the original property should be called. Defaults <c>true</c></param>
    /// <returns>The original property plug instance</returns>
    IPropertyPlug<TProperty> CallOriginal(bool shouldCallOriginal = true);
}
