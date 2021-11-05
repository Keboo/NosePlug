using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug
{
    public class Nasal
    {
        private List<IPlug> Plugs { get; } = new();

        public Nasal PlugProperty<T, TReturn>(
            string name, Func<TReturn> getReturnValue)
        {
            string id = GetId<T>(name);
            var property = typeof(T).GetProperty(name);
            MethodInfo origianl = property!.GetGetMethod()!;

            Smell smell = GetCodeSmell(origianl, id);
            IPlug plug = smell.Plug<TReturn>(getReturnValue);
            Plugs.Add(plug);

            return this;
        }

        public Nasal PlugMethod<TReturn>
            (Expression<Action> methodExpression, Func<TReturn> getReturnValue)
        {
            var methodCallExpression = methodExpression.Body as MethodCallExpression;
            if (methodCallExpression is null)
            {
                throw new ArgumentException();
            }
            MethodInfo origianl = methodCallExpression.Method;
            string id = GetId(origianl.Name, origianl.DeclaringType!);
            Smell smell = GetCodeSmell(origianl, id);

            IPlug plug = smell.Plug(getReturnValue);
            Plugs.Add(plug);

            return this;
        }

        private static string GetId<T>(string name) => GetId(name, typeof(T));
        private static string GetId(string name, Type type) => $"noseplug.{type.Name}.{name}";

        private static Smell GetCodeSmell(MethodInfo method, string id)
        {
            var instance = new Harmony(id);

            var processor = instance.CreateProcessor(method);

            Smell smell = new(processor, id, method);
            return smell;
        }

        public async Task<IDisposable> ApplyAsync()
        {
            foreach(var plug in Plugs)
            {
                await plug.PatchAsync();
            }
            return new NoseCleaner(this);
        }

        private class NoseCleaner : IDisposable
        {
            private Nasal Nasal { get; }

            public NoseCleaner(Nasal nasal)
            {
                Nasal = nasal ?? throw new ArgumentNullException(nameof(nasal));
            }

            public void Dispose()
            {
                foreach(var plug in Nasal.Plugs)
                {
                    plug.Dispose();
                }
            }
        }
    }
}
