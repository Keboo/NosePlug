namespace NosePlug.Plugs;

internal interface IMethodHandler : IDisposable
{
    void Patch(PatchProcessor processor);
    bool ShouldCallOriginal { get; set; }

    void AssertMatches(MethodInfo original);
}
