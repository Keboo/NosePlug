using System.Reflection;

namespace NosePlug.Plugs;

internal partial class MethodPlug : BaseMethodPlug, IMethodPlug
{
    public MethodPlug(MethodInfo original) : base(original)
    {
    }

    IMethodPlug IMethodPlug.CallOriginal(bool shouldCallOriginal)
    {
        ShouldCallOriginal = shouldCallOriginal;
        return this;
    }
}

internal partial class MethodPlug<TReturn> : BaseMethodPlug, IMethodPlug<TReturn>
{
    public MethodPlug(MethodInfo original) : base(original)
    {
    }

    IMethodPlug<TReturn> IMethodPlug<TReturn>.CallOriginal(bool shouldCallOriginal)
    {
        ShouldCallOriginal = shouldCallOriginal;
        return this;
    }
}
