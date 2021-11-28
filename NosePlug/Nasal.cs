using NosePlug.Plugs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NosePlug;

/// <summary>
/// The main entry point for creating plugs.
/// </summary>
public class Nasal
{
    private List<IPlug> Plugs { get; } = new();
    private List<IPlug> Patched { get; } = new();

    /// <summary>
    /// Creates a plug for a property
    /// </summary>
    /// <typeparam name="TProperty">The return type of the property</typeparam>
    /// <param name="property">The <see cref="PropertyInfo"/> to create a plug for</param>
    /// <returns>A new property plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="PropertyInfo"/> is <c>null</c></exception>
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

    /// <summary>
    /// Create a method plug for a void returning method
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
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

    /// <summary>
    /// Create a method plug for a method with a return value
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method</typeparam>
    /// <param name="method">The <see cref="MethodInfo"/> to create a plug for</param>
    /// <returns>A new method plug</returns>
    /// <exception cref="ArgumentNullException">When the passed in <see cref="MethodInfo"/> is <c>null</c></exception>
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

    /// <summary>
    /// Applies all plugs and returns a disposable scope.
    /// This scope should be disposed when the plugs are no longer needed (typically at the end of a test).
    /// Only a single Nasel instance can be applied at a time. 
    /// If multiple calls to ApplyAsync occur, subsequent ones will block until a 
    /// lock on all plugged methods and properties can be obtained.
    /// </summary>
    /// <returns>A disposable scope.</returns>
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
