using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NosePlug
{
    internal class PropertyPlug<TProperty> : Plug, INasalPropertyPlug<TProperty>
    {
        private static Dictionary<InterceptorKey, Func<TProperty>> GetterCallbacks { get; } = new();
        private static Dictionary<InterceptorKey, Action<TProperty>> SetterCallbacks { get; } = new();

        private static MethodInfo GetterPrefixInfo { get; }
            = typeof(PropertyPlug<TProperty>).GetMethod(nameof(GetterPrefix)) ?? throw new MissingMethodException();

        private static MethodInfo SetterPrefixInfo { get; }
            = typeof(PropertyPlug<TProperty>).GetMethod(nameof(SetterPrefix)) ?? throw new MissingMethodException();

        protected override InterceptorKey Key { get; }
        public PropertyInfo Property { get; }

        private PatchProcessor? GetterProcessor { get; set; }
        public Func<TProperty>? Getter { get; set; }
        private PatchProcessor? SetterProcessor { get; set; }
        public Action<TProperty>? Setter { get; set; }

        public PropertyPlug(PropertyInfo property)
            : base($"noseplug.{property.FullDescription()}")
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Key = InterceptorKey.FromProperty(property);
        }

        public override void Patch()
        {
            if (Getter is not null)
            {
                var instance = new Harmony(Id + "_get");
                GetterProcessor = instance.CreateProcessor(Property.GetMethod);
                GetterProcessor.AddPrefix(GetterPrefixInfo);

                lock (GetterCallbacks)
                {
                    GetterCallbacks[InterceptorKey.FromMethod(Property.GetMethod!)] = Getter;
                }
                _ = GetterProcessor!.Patch();
            }
            if (Setter is not null)
            {
                var instance = new Harmony(Id + "_set");
                SetterProcessor = instance.CreateProcessor(Property.SetMethod);
                SetterProcessor.AddPrefix(SetterPrefixInfo);

                lock (SetterCallbacks)
                {
                    SetterCallbacks[InterceptorKey.FromMethod(Property.SetMethod!)] = Setter;
                }
                _ = SetterProcessor!.Patch();
            }
        }

        public override void Dispose()
        {
            if (GetterProcessor is { } getterProcessor)
            {
                lock (GetterCallbacks)
                {
                    GetterCallbacks.Remove(Key);
                }
                getterProcessor.Unpatch(HarmonyPatchType.All, Id + "_get");
            }
            if (SetterProcessor is { } setterProcessor)
            {
                lock (SetterCallbacks)
                {
                    SetterCallbacks.Remove(Key);
                }
                setterProcessor.Unpatch(HarmonyPatchType.All, Id + "_set");
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

        public INasalPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue)
        {
            if (!Property.CanRead)
            {
                throw new NasalException($"Property '{Property.DeclaringType?.FullName}.{Property.Name}' does not have a getter");
            }
            Getter = getReturnValue ?? throw new ArgumentNullException(nameof(getReturnValue));
            return this;
        }

        public INasalPropertyPlug<TProperty> ReplaceSetter(Action<TProperty> newSetter)
        {
            if (!Property.CanWrite)
            {
                throw new NasalException($"Property '{Property.DeclaringType?.FullName}.{Property.Name}' does not have a setter");
            }
            Setter = newSetter ?? throw new ArgumentNullException(nameof(newSetter));
            return this;
        }

        public INasalPropertyPlug<TProperty> CallBase(bool shouldCallBase = true)
        {
            return this;
        }
    }
}
