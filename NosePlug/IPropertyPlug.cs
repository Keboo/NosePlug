using System;

namespace NosePlug;

public interface IPropertyPlug<TProperty>
{
    IPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue);

    IPropertyPlug<TProperty> Callback(Action<TProperty> newSetter);
    IPropertyPlug<TProperty> CallOriginal(bool shouldCallOriginal = true);
}
