using System;
using System.Threading.Tasks;

namespace NosePlug
{
    public interface INasalMethodPlug
    {
        INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue);

        INasalMethodPlug Callback(Action callback);
        INasalMethodPlug Callback(Func<Task> callback);
        INasalMethodPlug Callback<T1, T2>(Action<T1, T2> callback);
    }
}
