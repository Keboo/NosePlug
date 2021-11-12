using NosePlug.Plugs;
using System;
using System.Collections.Generic;
using System.Linq;
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

            PropertyPlug<TProperty> plug = new(property);
            Plugs.Add(plug);
            return plug;
        }

        public INasalMethodPlug Method(MethodInfo method)
        {
            MethodPlug plug = new(method);
            Plugs.Add(plug);
            return plug;
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

        private sealed class NoseCleaner : IDisposable
        {
            private bool disposedValue;

            private Nasal Nasal { get; }

            public NoseCleaner(Nasal nasal)
            {
                Nasal = nasal ?? throw new ArgumentNullException(nameof(nasal));
            }

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        foreach (var plug in Nasal.Plugs.OrderByDescending(x => x.Id))
                        {
                            plug.Dispose();
                        }
                    }

                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
