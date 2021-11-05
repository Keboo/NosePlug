using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NosePlug
{
    public class Nasal
    {
        public void PlugProperty<TReturn>(
            string name, Func<TReturn> getReturnValue)
        {
        }

        public void PlugMethod<TReturn>
            (Expression<Action> methodExpression, Func<TReturn> getReturnValue)
        {
        }

        public Task<IDisposable> ApplyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
