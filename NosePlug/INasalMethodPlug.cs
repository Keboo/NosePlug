using System;

namespace NosePlug
{
    public interface INasalMethodPlug
    {
        INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue);

        INasalMethodPlug Callback(Action callback);
    }
}
