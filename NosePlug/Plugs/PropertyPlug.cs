
/* Unmerged change from project 'NosePlug (net48)'
Before:
using HarmonyLib;
using System;
After:
using System;
*/

/* Unmerged change from project 'NosePlug (net472)'
Before:
using HarmonyLib;
using System;
After:
using System;
*/

/* Unmerged change from project 'NosePlug (netcoreapp3.1)'
Before:
using HarmonyLib;
using System;
After:
using System;
*/

/* Unmerged change from project 'NosePlug (net48)'
Before:
using System.Reflection;
After:
using System.Reflection;
using HarmonyLib;
*/

/* Unmerged change from project 'NosePlug (net472)'
Before:
using System.Reflection;
After:
using System.Reflection;
using HarmonyLib;
*/

/* Unmerged change from project 'NosePlug (netcoreapp3.1)'
Before:
using System.Reflection;
After:
using System.Reflection;
using HarmonyLib;
*/
namespace NosePlug.Plugs;

internal class PropertyPlug<TProperty> : Plug, IPropertyPlug<TProperty>
{
    private static Dictionary<InterceptorKey, PropertyPlug<TProperty>> Callbacks { get; } = new();

    private static MethodInfo GetterPrefixInfo { get; }
        = typeof(PropertyPlug<TProperty>).GetMethod(nameof(GetterPrefix)) ?? throw new MissingMethodException();

    private static MethodInfo SetterPrefixInfo { get; }
        = typeof(PropertyPlug<TProperty>).GetMethod(nameof(SetterPrefix)) ?? throw new MissingMethodException();

    protected override InterceptorKey Key { get; }
    private InterceptorKey GetterKey => InterceptorKey.FromMethod(Property.GetMethod!);
    private InterceptorKey SetterKey => InterceptorKey.FromMethod(Property.SetMethod!);
    public PropertyInfo Property { get; }

    private PatchProcessor? GetterProcessor { get; set; }
    public Func<TProperty>? Getter { get; set; }
    private PatchProcessor? SetterProcessor { get; set; }
    public Action<TProperty>? Setter { get; set; }

    private bool ShouldCallOriginal { get; set; }

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

            lock (Callbacks)
            {
                Callbacks[GetterKey] = this;
            }
            _ = GetterProcessor!.Patch();
        }
        if (Setter is not null)
        {
            var instance = new Harmony(Id + "_set");
            SetterProcessor = instance.CreateProcessor(Property.SetMethod);
            SetterProcessor.AddPrefix(SetterPrefixInfo);

            lock (Callbacks)
            {
                Callbacks[SetterKey] = this;
            }
            _ = SetterProcessor!.Patch();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (GetterProcessor is { } getterProcessor)
            {
                lock (Callbacks)
                {
                    Callbacks.Remove(GetterKey);
                }
                getterProcessor.Unpatch(HarmonyPatchType.All, Id + "_get");
            }
            if (SetterProcessor is { } setterProcessor)
            {
                lock (Callbacks)
                {
                    Callbacks.Remove(SetterKey);
                }
                setterProcessor.Unpatch(HarmonyPatchType.All, Id + "_set");
            }
        }
        base.Dispose(disposing);
    }

    public static bool SetterPrefix(MethodBase __originalMethod, TProperty __0)
    {
        bool gotSetter;
        PropertyPlug<TProperty>? plug;
        lock (Callbacks)
        {
            gotSetter = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out plug);
        }
        if (gotSetter && plug is not null)
        {
            if (plug.Setter is { } setter)
            {
                setter(__0);
            }
            return plug.ShouldCallOriginal;
        }
        return true;
    }

    public static bool GetterPrefix(MethodBase __originalMethod, ref TProperty __result)
    {
        bool gotGetter;
        PropertyPlug<TProperty>? plug;
        lock (Callbacks)
        {
            gotGetter = Callbacks.TryGetValue(InterceptorKey.FromMethod(__originalMethod), out plug);
        }
        if (gotGetter && plug is not null)
        {
            if (plug.Getter is { } getter)
            {
                __result = getter();
            }
            return plug.ShouldCallOriginal;
        }
        return true;
    }

    public IPropertyPlug<TProperty> Returns(Func<TProperty> getReturnValue)
    {
        if (!Property.CanRead)
        {
            throw new NasalException($"Property '{Property.DeclaringType?.FullName}.{Property.Name}' does not have a getter");
        }
        Getter = getReturnValue ?? throw new ArgumentNullException(nameof(getReturnValue));
        return this;
    }

    public IPropertyPlug<TProperty> Callback(Action<TProperty> newSetter)
    {
        if (!Property.CanWrite)
        {
            throw new NasalException($"Property '{Property.DeclaringType?.FullName}.{Property.Name}' does not have a setter");
        }
        Setter = newSetter ?? throw new ArgumentNullException(nameof(newSetter));
        return this;
    }

    public IPropertyPlug<TProperty> CallOriginal(bool shouldCallOriginal = true)
    {
        ShouldCallOriginal = shouldCallOriginal;
        Setter ??= x => { };
        return this;
    }
}
