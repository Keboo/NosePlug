using System.Runtime.CompilerServices;

namespace NosePlug.Tests.TestClasses;

internal class HasPrivateSetter
{
#if NET472 || NET48
    public static Guid Foo { get; [MethodImpl(MethodImplOptions.NoInlining)] private set; }
#else
    public static Guid Foo { get; private set; }
#endif
    public static void SetProperty(Guid value) => Foo = value;
}
