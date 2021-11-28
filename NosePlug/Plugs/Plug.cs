using System;
using System.Threading.Tasks;

namespace NosePlug.Plugs;

internal abstract class Plug : IPlug
{
    private bool _isDisposed;
    protected bool IsDisposed => _isDisposed;

    public Plug(string id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Id { get; }
    protected abstract InterceptorKey Key { get; }

    public async Task AcquireLockAsync() => await Key.LockAsync();

    public abstract void Patch();

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                Key.Unlock();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
