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

        public INasalMethodPlug Callback(Action callback)
        {
            if (Original.ReturnType == typeof(void))
            {
                MethodHandler = new VoidMethodHandler(Key, callback);
            }
            else
            {
                MethodHandler = new DefaultMethodReturnHandler(Key, callback);
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
    }
}
