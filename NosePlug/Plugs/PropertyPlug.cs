using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class PropertyPlug<TProperty> : Plug
    {
        private static Dictionary<InterceptorKey, Func<TProperty>> GetterCallbacks { get; } = new();
        private static Dictionary<InterceptorKey, Action<TProperty>> SetterCallbacks { get; } = new();

        private static MethodInfo GetterPrefixInfo { get; }
            = typeof(PropertyPlug<TProperty>).GetMethod(nameof(GetterPrefix)) ?? throw new MissingMethodException();

        private static MethodInfo SetterPrefixInfo { get; }
            = typeof(PropertyPlug<TProperty>).GetMethod(nameof(SetterPrefix)) ?? throw new MissingMethodException();

        protected override InterceptorKey Key { get; }

        private PatchProcessor? GetterProcessor { get; }
        public PropertyInfo Property { get; }
        public Func<TProperty>? Getter { get; }
        private PatchProcessor? SetterProcessor { get; }
        public Action<TProperty>? Setter { get; }

        public PropertyPlug(
            PropertyInfo property,
            Func<TProperty>? getter,
            Action<TProperty>? setter)
            : base($"noseplug.{property.FullDescription()}")
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Key = InterceptorKey.FromProperty(property);
            Getter = getter;
            if (Getter is not null)
            {
                var instance = new Harmony(Id + "_get");
                GetterProcessor = instance.CreateProcessor(property.GetMethod);
                GetterProcessor.AddPrefix(GetterPrefixInfo);
            }

            Setter = setter;
            if (Setter is not null)
            {
                var instance = new Harmony(Id + "_set");
                SetterProcessor = instance.CreateProcessor(property.SetMethod);
                SetterProcessor.AddPrefix(SetterPrefixInfo);
            }
        }

        public override void Patch()
        {
            if (Getter is not null)
            {
                lock (GetterCallbacks)
                {
                    GetterCallbacks[InterceptorKey.FromMethod(Property.GetMethod!)] = Getter;
                }
                _ = GetterProcessor!.Patch();
            }
            if (Setter is not null)
            {
                lock (SetterCallbacks)
                {
                    SetterCallbacks[InterceptorKey.FromMethod(Property.SetMethod!)] = Setter;
                }
                _ = SetterProcessor!.Patch();
            }
        }

        public override void Dispose()
        {
            if (Getter is not null)
            {
                lock (GetterCallbacks)
                {
                    GetterCallbacks.Remove(Key);
                }
                GetterProcessor!.Unpatch(HarmonyPatchType.All, Id + "_get");
            }
            if (Setter is not null)
            {
                lock (SetterCallbacks)
                {
                    SetterCallbacks.Remove(Key);
                }
                SetterProcessor!.Unpatch(HarmonyPatchType.All, Id + "_set");
            }
            base.Dispose();
        }

        public static bool SetterPrefix(TProperty value, MethodBase __originalMethod)
        {
            bool gotSetter;
            Action<TProperty>? interceptor;
            lock (SetterCallbacks)
            {
                gotSetter = SetterCallbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out interceptor);
            }
            if (gotSetter && interceptor is not null)
            {
                interceptor(value);
                return false;
            }
            return true;
        }

        public static bool GetterPrefix(ref TProperty __result, MethodBase __originalMethod)
        {
            bool gotGetter;
            Func<TProperty>? interceptor;
            lock (GetterCallbacks)
            {
                gotGetter = GetterCallbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out interceptor);
            }
            if (gotGetter && interceptor is not null)
            {
                __result = interceptor();
                return false;
            }
            return true;
        }
    }
}
