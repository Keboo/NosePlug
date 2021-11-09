//using System;
//using System.Reflection;

//namespace NosePlug.Plugs
//{
//    internal sealed class VoidMethodHandler : BaseMethodHandler
//    {
//        protected override MethodInfo PrefixInfo { get; }
//            = typeof(VoidMethodHandler).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

//        private Action Callback { get; }

//        public VoidMethodHandler(InterceptorKey key, Action callback)
//             : base(key)
//        {
//            Callback = callback;
//        }

//        public static bool MethodWithReturnPrefix(MethodBase __originalMethod)
//        {
//            if (TryGetHandler(__originalMethod, out VoidMethodHandler? handler))
//            {
//                handler.Callback();
//                return false;
//            }
//            return true;
//        }
//    }

//    internal sealed class VoidMethodHandler<T1> : BaseMethodHandler
//    {
//        protected override MethodInfo PrefixInfo { get; }
//            = typeof(VoidMethodHandler<T1>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

//        private Action<T1> Callback { get; }

//        public VoidMethodHandler(InterceptorKey key, Action<T1> callback)
//             : base(key)
//        {
//            Callback = callback;
//        }

//        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, T1 __0)
//        {
//            if (TryGetHandler(__originalMethod, out VoidMethodHandler<T1>? handler))
//            {
//                handler.Callback(__0);
//                return false;
//            }
//            return true;
//        }
//    }

//    internal sealed class VoidMethodHandler<T1, T2> : BaseMethodHandler
//    {
//        protected override MethodInfo PrefixInfo { get; }
//            = typeof(VoidMethodHandler<T1, T2>).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

//        private Action<T1, T2> Callback { get; }

//        public VoidMethodHandler(InterceptorKey key, Action<T1, T2> callback)
//             : base(key)
//        {
//            Callback = callback;
//        }

//        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, T1 __0, T2 __1)
//        {
//            if (TryGetHandler(__originalMethod, out VoidMethodHandler<T1, T2>? handler))
//            {
//                handler.Callback(__0, __1);
//                return false;
//            }
//            return true;
//        }
//    }
//}
