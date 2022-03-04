using System.Runtime.CompilerServices;

namespace NosePlug.Tests.TestClasses;

internal class HasPublicProperty
{
#if NET472 || NET48
    public static Guid Foo { get; [MethodImpl(MethodImplOptions.NoInlining)] set; }
#else
    public static Guid Foo { get; set; }
#endif
}
