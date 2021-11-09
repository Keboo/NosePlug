using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal sealed class VoidMethodHandler : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(VoidMethodHandler).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Action Callback { get; }

        public VoidMethodHandler(InterceptorKey key, Action callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod)
        {
            if (TryGetHandler(__originalMethod, out VoidMethodHandler? handler))
            {
                handler.Callback();
                return false;
            }
            return true;
        }
    }

    internal sealed class VoidMethodHandler<T1> : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(VoidMethodHandler<T1>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Action<T1> Callback { get; }

        public VoidMethodHandler(InterceptorKey key, Action<T1> callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, T1 __0)
        {
            if (TryGetHandler(__originalMethod, out VoidMethodHandler<T1>? handler))
            {
                handler.Callback(__0);
                return false;
            }
            return true;
        }
    }

    internal sealed class VoidMethodHandler<T1, T2> : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(VoidMethodHandler<T1, T2>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Action<T1, T2> Callback { get; }

        public VoidMethodHandler(InterceptorKey key, Action<T1, T2> callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, T1 __0, T2 __1)
        {
            if (TryGetHandler(__originalMethod, out VoidMethodHandler<T1, T2>? handler))
            {
                handler.Callback(__0, __1);
                return false;
            }
            return true;
        }
    }

    internal class MethodHandler_old : IMethodHandler
    {
        private static MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler_old).GetMethod(nameof(MethodPrefix)) ?? throw new MissingMethodException();

        private static Dictionary<InterceptorKey, Action> Callbacks { get; } = new();

        private InterceptorKey Key { get; }

        private Action Interceptor { get; }

        public MethodHandler_old(InterceptorKey key, Action interceptor)
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
