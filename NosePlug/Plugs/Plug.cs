using System;
using System.Threading.Tasks;

namespace NosePlug
{

    internal abstract class Plug : IPlug
    {
        public Plug(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string Id { get; }
        protected abstract InterceptorKey Key { get; }

        public async Task AcquireLockAsync() => await Key.LockAsync();

        public abstract void Patch();

        public virtual void Dispose()
        {
            Key.Unlock();
        }
}
}
