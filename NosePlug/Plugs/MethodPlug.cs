using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class MethodPlug<TReturn> : Plug
    {
        private static Dictionary<InterceptorKey, Func<TReturn>> Callbacks { get; } = new();

        private static MethodInfo PrefixInfo { get; }
            = typeof(MethodPlug<TReturn>).GetMethod(nameof(Prefix)) ?? throw new MissingMethodException();

        protected override InterceptorKey Key { get; }
        private PatchProcessor Processor { get; }
        private Func<TReturn> Interceptor { get; }

        public MethodPlug(
            MethodBase original,
            Func<TReturn> prefix)
            : base($"noseplug.{original.FullDescription()}")
        {
            Key = InterceptorKey.FromMethod(original ?? throw new ArgumentNullException(nameof(original)));
            Interceptor = prefix ?? throw new ArgumentNullException(nameof(prefix));
            
            var instance = new Harmony(Id);
            Processor = instance.CreateProcessor(original);
            Processor.AddPrefix(PrefixInfo);
        }

        public override void Patch()
        {
            lock (Callbacks)
            {
                Callbacks[Key] = Interceptor;
            }
            _ = Processor.Patch();
        }

        public override void Dispose()
        {
            lock (Callbacks)
            {
                Callbacks.Remove(Key);
            }
            Processor.Unpatch(HarmonyPatchType.All, Id);
            base.Dispose();
        }

        public static bool Prefix(ref TReturn __result, MethodBase __originalMethod)
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
}
