using System;
using System.Threading.Tasks;

namespace NosePlug.Plugs
{
    internal interface IPlug : IDisposable
    {
        string Id { get; }
        Task AcquireLockAsync();
        void Patch();
    }
}
