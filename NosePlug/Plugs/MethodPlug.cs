using HarmonyLib;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug.Plugs
{
    internal partial class MethodPlug : Plug, INasalMethodPlug
    {
        protected override InterceptorKey Key { get; }
        private PatchProcessor? Processor { get; set; }

        internal IMethodHandler? MethodHandler { get; set; }
        private MethodInfo Original { get; }

        public MethodPlug(MethodInfo original)
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
            if (Original.ReturnType == typeof(Task<TReturn>))
            {
                MethodHandler = new MethodHandler<Task<TReturn>>(Key, () => Task.FromResult(getReturnValue()));
            }
            else
            {
                MethodHandler = new MethodHandler<TReturn>(Key, getReturnValue);
            }
            return this;
        }

        public INasalMethodPlug Callback(Func<Task> callback)
        {
            MethodHandler = new MethodHandler<Task>(Key, async () =>
            {
                await callback();
            });

            return this;
        }

        private static object? GetDefaultValue(Type type)
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
}
