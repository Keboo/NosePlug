using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    public void AssertMatches(MethodInfo original)
    {
        var originalParameters = original.GetParameters();
        var parameters = Key.ParameterTypes.ToArray();

        bool matches = true;
        if (originalParameters.Length == parameters.Length)
        {
            for (int i = 0; i < parameters.Length && matches; i++)
            {
                matches = originalParameters[i].ParameterType == parameters[i] ||
                    originalParameters[i].ParameterType.IsSubclassOf(parameters[i]);
            }
        }
        else
        {
            matches = false;
        }

        if (!matches)
        {
            throw new NasalException($"Plug for {original.DeclaringType.FullName}.{original.Name} has callback parameters ({GetTypeDisplay(parameters)}) that do not match original method parameters ({GetParametersDisplay(originalParameters)})");
        }

        static string GetParametersDisplay(ParameterInfo[] parameters)
            => GetTypeDisplay(parameters.Select(x => x.ParameterType));

        static string GetTypeDisplay(IEnumerable<Type> types)
        {
            var rv = string.Join(", ", types.Select(x => x.FullName));
            return rv.Length == 0 ? "<empty>" : rv;
        }
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
