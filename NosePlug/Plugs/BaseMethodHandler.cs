using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NosePlug.Plugs;

internal abstract class BaseMethodHandler : IMethodHandler
{
    private bool _isDisposed;

    protected abstract MethodInfo PrefixInfo { get; }

    private static Dictionary<InterceptorKey, IMethodHandler> Callbacks { get; } = new();

    private InterceptorKey Key { get; }

    public bool ShouldCallOriginal { get; set; }

    public BaseMethodHandler(InterceptorKey key)
    {
        Key = key;
    }

    public void Patch(PatchProcessor processor)
    {
        lock (Callbacks)
        {
            Callbacks[Key] = this;
        }
        processor.AddPrefix(PrefixInfo);
        _ = processor.Patch();
    }

    protected static bool TryGetHandler<THandler>(MethodBase originalMethod,
        [NotNullWhen(true)] out THandler? handler)
        where THandler : IMethodHandler
    {
        bool gotValue;
        IMethodHandler? methodHandler;
        lock (Callbacks)
        {
            gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(originalMethod), out methodHandler);
        }
        if (gotValue && methodHandler is THandler typedHandler)
        {
            handler = typedHandler;
            return true;
        }
        handler = default;
        return false;
    }

    protected void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                lock (Callbacks)
                {
                    Callbacks.Remove(Key);
                }
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}
