using System;

namespace NosePlug
{
    public interface INasalPropertyPlug<TProperty>
    {
        INasalPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue);

        INasalPropertyPlug<TProperty> ReplaceSetter(Action<TProperty> newSetter);
        INasalPropertyPlug<TProperty> CallOriginal(bool shouldCallBase = true);
    }
}
