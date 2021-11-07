using System;
using System.Threading.Tasks;

namespace NosePlug
{
    internal interface IPlug : IDisposable
    {
        string Id { get; }
        Task AcquireLockAsync();
        void Patch();
    }
}
