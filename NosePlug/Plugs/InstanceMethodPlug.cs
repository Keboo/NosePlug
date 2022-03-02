using System.Reflection;

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
