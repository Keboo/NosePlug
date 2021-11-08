using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal interface IMethodHandler : IDisposable
    {
        void Patch(PatchProcessor processor);
    }

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
            Func<TReturn>? interceptor;
            lock (Callbacks)
            {
                gotValue = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out interceptor);
            }
            if (gotValue && interceptor is not null)
            {
                __result = interceptor();
                return false;
            }
            return true;
        }
    }

    internal class MethodPlug : Plug, INasalMethodPlug
    {
        protected override InterceptorKey Key { get; }
        private PatchProcessor? Processor { get; set; }

        private IMethodHandler? MethodHandler { get; set; }
        private MethodBase Original { get; }

        public MethodPlug(MethodBase original)
            : base($"noseplug.{original.FullDescription()}")
        {
            Original = original ?? throw new ArgumentNullException(nameof(original));
            Key = InterceptorKey.FromMethod(original);
        }

        public override void Patch()
        {
            var instance = new Harmony(Id);
            var processor = Processor = instance.CreateProcessor(Original);
            MethodHandler?.Patch(processor);
        }

        public override void Dispose()
        {
            if (Processor is { } processor)
            {
                processor.Unpatch(HarmonyPatchType.All, Id);
            }
            MethodHandler?.Dispose();

            base.Dispose();
        }

        public INasalMethodPlug Returns<TReturn>(Func<TReturn> getReturnValue)
        {
            MethodHandler = new MethodHandler<TReturn>(Key, getReturnValue);
            return this;
        }

        public INasalMethodPlug Callback(Action callback)
        {
            MethodHandler = new MethodHandler(Key, callback);
            return this;
        }
    }
}
