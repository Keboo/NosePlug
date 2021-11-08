using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class MethodHandler : IMethodHandler
    {
        private static MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler).GetMethod(nameof(MethodPrefix)) ?? throw new MissingMethodException();

        private static Dictionary<InterceptorKey, Action> Callbacks { get; } = new();

        private InterceptorKey Key { get; }

        private Action Interceptor { get; }

        public MethodHandler(InterceptorKey key, Action interceptor)
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

        public static bool MethodPrefix(MethodBase __originalMethod)
        {
            bool gotValue;
            Action? interceptor;
            lock (Callbacks)
            {
                gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out interceptor);
            }
            if (gotValue && interceptor is not null)
            {
                interceptor();
                return false;
            }
            return true;
        }
    }
}
