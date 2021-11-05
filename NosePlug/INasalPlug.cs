using System;

namespace NosePlug
{
    public interface INasalPlug
    {
        void Returns<TReturn>(Func<TReturn> getReturnValue);
    }
}
