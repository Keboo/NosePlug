using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NosePlug.Plugs
{
    internal abstract class BaseMethodHandler : IMethodHandler
    {
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

        public void Dispose()
        {
            lock (Callbacks)
            {
                Callbacks.Remove(Key);
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
    }
}
