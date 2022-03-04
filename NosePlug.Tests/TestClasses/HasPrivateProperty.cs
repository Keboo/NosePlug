using System.Runtime.CompilerServices;

namespace NosePlug.Tests.TestClasses;

internal class HasPrivateProperty
{
#if NET472 || NET48
    private static Guid Foo { get; [MethodImpl(MethodImplOptions.NoInlining)] set; }
#else
    private static Guid Foo { get; set; }
#endif
    public static Guid ReadPrivateProperty() => Foo;
    public static Guid WritePrivateProperty(Guid value) => Foo = value;
}
