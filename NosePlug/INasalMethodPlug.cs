using System;
using System.Threading.Tasks;

namespace NosePlug
{
    public interface INasalMethodPlug
    {
        INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue);

        INasalMethodPlug Callback(Action callback);
        INasalMethodPlug Callback(Action<object[]> callback);
        INasalMethodPlug Callback(Func<Task> callback);

    }
}
