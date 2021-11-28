using HarmonyLib;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug.Plugs;

internal class BaseMethodPlug : Plug
{
    protected override InterceptorKey Key { get; }
    private PatchProcessor? Processor { get; set; }

    internal IMethodHandler? MethodHandler { get; set; }
    private protected MethodInfo Original { get; }

    private protected bool ShouldCallOriginal { get; set; }

    public BaseMethodPlug(MethodInfo original)
        : base($"noseplug.{original.FullDescription()}")
    {
        Original = original ?? throw new ArgumentNullException(nameof(original));
        Key = InterceptorKey.FromMethod(original);
    }

    public override void Patch()
    {
        var instance = new Harmony(Id);
        var processor = Processor = instance.CreateProcessor(Original);
        if (MethodHandler is { } methodHandler)
        {
            methodHandler.ShouldCallOriginal = ShouldCallOriginal;
            methodHandler.Patch(processor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Processor is { } processor)
            {
                processor.Unpatch(HarmonyPatchType.All, Id);
            }
            MethodHandler?.Dispose();
        }
        base.Dispose(disposing);
    }

    private protected static object? GetDefaultValue(Type type)
    {
        if (type == typeof(void))
        {
            return null;
        }
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        if (type == typeof(Task))
        {
            return Task.CompletedTask;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            Type taskType = type.GetGenericArguments()[0];
            object? taskDefaultValue = GetDefaultValue(taskType);
            //TODO: Cache
            return typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(taskType)
                .Invoke(null, new[] { taskDefaultValue });
        }
        return null;
    }
}
