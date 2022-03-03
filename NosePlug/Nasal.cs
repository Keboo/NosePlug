namespace NosePlug;

/// <summary>
/// The main entry point for creating plugs.
/// </summary>
public static partial class Nasal
{
    /// <summary>
    /// Applies all plugs and returns a disposable scope.
    /// This scope should be disposed when the plugs are no longer needed (typically at the end of a test).
    /// If multiple calls to ApplyAsync occur, subsequent ones will block until a 
    /// lock on all plugged methods and properties can be obtained.
    /// </summary>
    /// <returns>A disposable scope.</returns>
    public static async Task<IDisposable> ApplyAsync(params IPlug[] plugs)
    {
        if (plugs.Length == 0)
        {
            throw new ArgumentException("At least one plug must be specified", nameof(plugs));
        }

        List<Plugs.IPlug> internalPlugs = new(plugs.Length);
        foreach (var plug in plugs)
        {
            if (plug is Plugs.IPlug internalPlug)
            {
                internalPlugs.Add(internalPlug);
            }
            else
            {
                throw new ArgumentException($"Plug {plug.GetType().FullName} is not a valid {typeof(IPlug).FullName}", nameof(plugs));
            }
        }

        foreach (var plug in internalPlugs.OrderBy(x => x.Id))
        {
            await plug.AcquireLockAsync();
        }

        var rv = new NoseCleaner(internalPlugs);
        try
        {
            //Ordering here not strictly necessary since we have acquired all locks
            foreach (var plug in internalPlugs.OrderBy(x => x.Id))
            {
                plug.Patch();
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
        private bool _isDisposed;

        public IReadOnlyList<Plugs.IPlug> Plugs { get; }

        public NoseCleaner(IReadOnlyList<Plugs.IPlug> plugs)
        {
            Plugs = plugs;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    foreach (var plug in Plugs.OrderByDescending(x => x.Id))
                    {
                        plug.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
