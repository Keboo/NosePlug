using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug
{
    public class Nasal
    {
        private List<IPlug> Plugs { get; } = new();

        public INasalPlug Property<T>(string name)
        {
            var property = typeof(T).GetProperty(name);
            MethodInfo origianl = property!.GetGetMethod()!;

            Smell smell = GetCodeSmell(origianl);

            return new NasalPlug(this, smell);
        }

        public INasalPlug Method(Expression<Action> methodExpression)
        {
            var methodCallExpression = methodExpression.Body as MethodCallExpression;
            if (methodCallExpression is null)
            {
                throw new ArgumentException();
            }
            MethodInfo origianl = methodCallExpression.Method;
            Smell smell = GetCodeSmell(origianl);
            return new NasalPlug(this, smell);
        }

        private static Smell GetCodeSmell(MethodInfo method)
        {
            string id = $"noseplug.{method.FullDescription()}";
            var instance = new Harmony(id);

            var processor = instance.CreateProcessor(method);

            Smell smell = new(processor, id, method);
            return smell;
        }

        public async Task<IDisposable> ApplyAsync()
        {
            foreach (var plug in Plugs.OrderBy(x => x.Id))
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
                foreach (var plug in Nasal.Plugs.OrderByDescending(x => x.Id))
                {
                    plug.Dispose();
                }
            }
        }

        internal class NasalPlug : INasalPlug
        {
            private Nasal Nasal { get; }
            private Smell Smell { get; }

            public NasalPlug(Nasal nasal, Smell smell)
            {
                Nasal = nasal;
                Smell = smell;
            }

            public void Returns<TReturn>(Func<TReturn> getReturnValue)
            {
                IPlug plug = Smell.Plug(getReturnValue);
                Nasal.Plugs.Add(plug);
            }
        }
    }
}
