using HarmonyLib;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug
{
    public static class Smell<T>
    {
        public static async Task<IDisposable> PlugProperty<TReturn>(
            string name, Func<TReturn> getReturnValue)
        {
            string id = $"noseplug.{typeof(T).Name}.{name}";
            var targetType = typeof(T);
            var instance = new Harmony(id);

            var property = targetType.GetProperty(name);
            MethodBase origianl = property!.GetGetMethod();
            var processor = instance.CreateProcessor(origianl);

            Smell smell = new(processor, id, origianl);
            Plug<TReturn> plug = smell.Plug(getReturnValue);

            await plug.PatchAsync();
            return plug;
        }

        public static async Task<IDisposable> PlugMethod<TReturn>
            (Expression<Action> methodExpression, Func<TReturn> getReturnValue)
        {
            var methodCallExpression = methodExpression.Body as MethodCallExpression;
            if (methodCallExpression is null)
            {
                throw new ArgumentException();
            }
            MethodInfo method = methodCallExpression.Method;
            string name = method.Name;
            string id = $"noseplug.{typeof(T).Name}.{name}";
            var targetType = typeof(T);
            var instance = new Harmony(id);

            var processor = instance.CreateProcessor(method);

            Smell smell = new(processor, id, method);
            Plug<TReturn> plug = smell.Plug(getReturnValue);

            await plug.PatchAsync();
            return plug;
        }
    }
}
