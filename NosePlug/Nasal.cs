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

        public INasalPropertyPlug<TProperty> Property<TProperty>(PropertyInfo property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return new NasalPropertyPlug<TProperty>(this, property);
        }

        public INasalPlug GetPlug(MethodInfo method)
        {
            return new NasalMethodPlug(this, method);
        }

        public async Task<IDisposable> ApplyAsync()
        {
            foreach (var plug in Plugs.OrderBy(x => x.Id))
            {
                await plug.AcquireLockAsync();
            }

            //Ordering here not strictly neccisary since we have acquired all locks
            foreach (var plug in Plugs.OrderBy(x => x.Id))
            {
                plug.Patch();
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

        internal abstract class NasalPlug
        {
            public Nasal Nasal { get; }

            public NasalPlug(Nasal nasal)
            {
                Nasal = nasal;
            }
        }

        internal class NasalMethodPlug : NasalPlug, INasalPlug
        {
            public NasalMethodPlug(Nasal nasal, MethodInfo method)
                : base(nasal)
            {
                Method = method ?? throw new ArgumentNullException(nameof(method));
            }

            public MethodInfo Method { get; }

            public void Returns<TReturn>(Func<TReturn> getReturnValue)
            {
                IPlug plug = new MethodPlug<TReturn>(Method, getReturnValue);
                Nasal.Plugs.Add(plug);
            }

        }

        internal class NasalPropertyPlug<TProperty> : NasalPlug, INasalPropertyPlug<TProperty>
        {
            public NasalPropertyPlug(Nasal nasal, PropertyInfo property)
                : base(nasal)
            {
                Property = property ?? throw new ArgumentNullException(nameof(property));
            }

            public PropertyInfo Property { get; }
            private PropertyPlug<TProperty>? Plug { get; set; }

            public void Returns(Func<TProperty> getReturnValue)
                => _ = GetPlug(getReturnValue, null);

            public void ReplaceSetter(Action<TProperty> newSetter) 
                => _ = GetPlug(null, newSetter);

            private IPlug GetPlug(Func<TProperty>? getter, Action<TProperty>? setter)
            {
                if (Plug is { } plug)
                {
                    Nasal.Plugs.Remove(plug);
                }

                plug = Plug = new PropertyPlug<TProperty>(Property, getter ?? Plug?.Getter, setter ?? Plug?.Setter);
                Nasal.Plugs.Add(plug);
                return plug;
            }
        }
    }
}
