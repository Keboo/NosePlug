using System;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPrivateSetter
    {
        public static Guid Foo { get; private set; }

        public static void SetPropety(Guid value) => Foo = value;
    }
}
