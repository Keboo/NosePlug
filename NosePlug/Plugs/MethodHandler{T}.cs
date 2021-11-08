using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class MethodHandler<TReturn> : IMethodHandler
    {
        private static MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler<TReturn>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private static Dictionary<InterceptorKey, Func<TReturn>> Callbacks { get; } = new();

        private InterceptorKey Key { get; }

        private Func<TReturn> Interceptor { get; }

        public MethodHandler(InterceptorKey key, Func<TReturn> interceptor)
        {
            Key = key;
            Interceptor = interceptor;
        }

        public void Patch(PatchProcessor processor)
        {
            lock (Callbacks)
            {
                Callbacks[Key] = Interceptor;
            }
            processor.AddPrefix(PrefixInfo);
            _ = processor.Patch();
        }

        public void Dispose()
        {
            lock (Callbacks)
            {
                Callbacks.Remove(Key);
            }
        }

        public static bool MethodWithReturnPrefix(ref TReturn __result, MethodBase __originalMethod)
        {
            bool gotValue;
            Func<TReturn>? callback;
            lock (Callbacks)
            {
                gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out callback);
            }
            if (gotValue && callback is not null)
            {
                __result = callback();
                return false;
            }
            return true;
        }
    }
}
