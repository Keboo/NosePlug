namespace NosePlug.Plugs;

internal partial class InstanceMethodPlug : BaseMethodPlug, IInstanceMethodPlug
{
    public InstanceMethodPlug(MethodInfo original) : base(original)
    {
    }

    IInstanceMethodPlug IInstanceMethodPlug.CallOriginal(bool shouldCallOriginal)
    {
        ShouldCallOriginal = shouldCallOriginal;
        return this;
    }
}

internal partial class InstanceMethodPlug<TReturn> : BaseMethodPlug, IInstanceMethodPlug<TReturn>
{
    public InstanceMethodPlug(MethodInfo original) : base(original)
    {
    }

    IInstanceMethodPlug<TReturn> IInstanceMethodPlug<TReturn>.CallOriginal(bool shouldCallOriginal)
    {
        ShouldCallOriginal = shouldCallOriginal;
        return this;
    }
}
