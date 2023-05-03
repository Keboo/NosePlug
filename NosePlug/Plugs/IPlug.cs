namespace NosePlug.Plugs;

internal interface IPlug : NosePlug.IPlug, IDisposable
{
    string Id { get; }
    Task AcquireLockAsync();
    void Patch();
}
