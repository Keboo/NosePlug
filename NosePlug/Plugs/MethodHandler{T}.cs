using System;
using System.Reflection;

namespace NosePlug.Plugs
{
    internal sealed class MethodHandler<T1, T2, TReturn> : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler<T1, T2, TReturn>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Func<T1, T2, TReturn> Callback { get; }

        public MethodHandler(InterceptorKey key, Func<T1, T2, TReturn> callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, ref TReturn __result, T1 __0, T2 __1)
        {
            if (TryGetHandler(__originalMethod, out MethodHandler<T1, T2, TReturn>? handler))
            {
                __result = handler.Callback(__0, __1);
                return false;
            }
            return true;
        }
    }

    internal sealed class MethodHandler<T1, TReturn> : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler<T1, TReturn>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Func<T1, TReturn> Callback { get; }

        public MethodHandler(InterceptorKey key, Func<T1, TReturn> callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, ref TReturn __result, T1 __0)
        {
            if (TryGetHandler(__originalMethod, out MethodHandler<T1, TReturn>? handler))
            {
                __result = handler.Callback(__0);
                return false;
            }
            return true;
        }
    }

    internal sealed class MethodHandler<TReturn> : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(MethodHandler<TReturn>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Func<TReturn> Callback { get; }

        public MethodHandler(InterceptorKey key, Func<TReturn> callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, ref TReturn __result)
        {
            if (TryGetHandler(__originalMethod, out MethodHandler<TReturn>? handler))
            {
                __result = handler.Callback();
                return false;
            }
            return true;
        }
    }
}
