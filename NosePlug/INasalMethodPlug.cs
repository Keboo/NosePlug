using System;
using System.Threading.Tasks;

namespace NosePlug
{
    public partial interface INasalMethodPlug
    {
        INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue);

        //INasalMethodPlug Callback(Func<Task> callback);
        //INasalMethodPlug Callback(Action callback);
        //INasalMethodPlug Callback<T1, T2>(Action<T1, T2> callback);
    }
}
