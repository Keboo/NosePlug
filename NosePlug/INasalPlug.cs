using System;

namespace NosePlug
{
    public interface INasalPlug
    {
        void Returns<TReturn>(Func<TReturn> getReturnValue);
    }

    public interface INasalPropertyPlug<TProperty>
    {
        void Returns(Func<TProperty> getReturnValue);

        void ReplaceSetter(Action<TProperty> newSetter);
    }
}
