using System;
using System.Threading.Tasks;

namespace NosePlug
{
    public partial interface INasalMethodPlug
    {
        INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue);
        INasalMethodPlug Callback(Func<Task> callback);
    }
}
