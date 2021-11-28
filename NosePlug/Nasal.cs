using NosePlug.Plugs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug;

public class Nasal
{
    private List<IPlug> Plugs { get; } = new();
    private List<IPlug> Patched { get; } = new();

    public IPropertyPlug<TProperty> Property<TProperty>(PropertyInfo property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        PropertyPlug<TProperty> plug = new(property);
        Plugs.Add(plug);
        return plug;
    }

    public IMethodPlug Method(MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        MethodPlug plug = new(method);
        Plugs.Add(plug);
        return plug;
    }

    public IMethodPlug<TReturn> Method<TReturn>(MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        MethodPlug<TReturn> plug = new(method);
        Plugs.Add(plug);
        return plug;
    }

    public async Task<IDisposable> ApplyAsync()
    {
        var rv = new NoseCleaner(this);
        foreach (var plug in Plugs.OrderBy(x => x.Id))
        {
            await plug.AcquireLockAsync();
        }

        try
        {
            //Ordering here not strictly necessary since we have acquired all locks
            foreach (var plug in Plugs.OrderBy(x => x.Id))
            {
                plug.Patch();
                Patched.Add(plug);
            }
        }
        catch (Exception)
        {
            rv.Dispose();
            throw;
        }
        return rv;
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
                    foreach (var plug in Nasal.Patched.OrderByDescending(x => x.Id))
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
