using NosePlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moq.AutoMock;

/// <summary>
/// A simple wrapper class for storing the list of plugs
/// </summary>
public class NasalPlugList
{
    /// <summary>
    /// A list of plugs.
    /// </summary>
    public IList<IPlug> Plugs { get; } = new List<IPlug>();

    /// <summary>
    /// Add a plug to the list.
    /// </summary>
    /// <param name="plug">The original plug that was passed in</param>
    public void Add(IPlug plug) => Plugs.Add(plug);

    /// <summary>
    /// Apply all plugs.
    /// </summary>
    /// <returns>A disposable scope.</returns>
    public Task<IDisposable> ApplyAsync() => Nasal.ApplyAsync(Plugs.ToArray());
}

