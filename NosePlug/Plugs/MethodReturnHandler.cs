using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class MethodReturnHandler : IMethodHandler
    {
        private static MethodInfo PrefixInfo { get; }
            = typeof(MethodReturnHandler).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private static Dictionary<InterceptorKey, MethodReturnHandler> Callbacks { get; } = new();

        private InterceptorKey Key { get; }

        private Func<object?> Callback { get; }

        public MethodReturnHandler(InterceptorKey key, Func<object?> callback)
        {
            Key = key;
            Callback = callback;
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

        public void Dispose()
        {
            lock (Callbacks)
            {
                Callbacks.Remove(Key);
            }
        }

        public static bool MethodWithReturnPrefix(ref object? __result, MethodBase __originalMethod)
        {
            bool gotValue;
            MethodReturnHandler handler;
            lock (Callbacks)
            {
                gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out handler);
            }
            if (gotValue && handler is not null)
            {
                __result = handler.Callback();
                return false;
            }
            return true;
        }
    }
}
