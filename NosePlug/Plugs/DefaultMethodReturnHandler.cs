using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug.Plugs
{
    internal class DefaultMethodReturnHandler : BaseMethodHandler
    {
        protected override MethodInfo PrefixInfo { get; }
            = typeof(DefaultMethodReturnHandler).GetMethod(nameof(MethodWithReturnPrefix)) ?? throw new MissingMethodException();

        private Action Callback { get; }

        public DefaultMethodReturnHandler(InterceptorKey key, Action callback)
             : base(key)
        {
            Callback = callback;
        }

        public static bool MethodWithReturnPrefix(MethodBase __originalMethod, ref object? __result)
        {
            if (TryGetHandler(__originalMethod, out DefaultMethodReturnHandler? handler))
            {
                handler.Callback();
                if (__originalMethod is MethodInfo method)
                {
                    __result = GetDefaultValue(method.ReturnType);
                }
                return false;
            }
            return true;
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
