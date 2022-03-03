using System.Runtime.CompilerServices;

namespace NosePlug.Tests.TestClasses;

internal class HasInstanceMethods
{
    public void VoidMethod()
    {
    }

    public void VoidMethodWithParameter(string myParameter)
    {

    }

    public void HasParameter(TestService service)
    {

    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private string HasHiddenType(HiddenType hiddenType)
        => "HasHiddenType";

    public string InvokeHasHiddenType()
        => HasHiddenType(new HiddenType());

    public int GetIntegerValue() => 42;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void NoParameters() { }
    public void InvokeNoParameters() => NoParameters();

    private class HiddenType
    { }
}
