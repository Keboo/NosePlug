using System;

namespace NosePlug
{
    public interface INasalPlug
    {
        void Returns<TReturn>(Func<TReturn> getReturnValue);
    }

    public interface INasalPropertyPlug<TProperty>
    {
        INasalPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue);

        INasalPropertyPlug<TProperty> ReplaceSetter(Action<TProperty> newSetter);
    }
}
