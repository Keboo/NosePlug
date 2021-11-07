using System;

namespace NosePlug.Tests.TestClasses
{
    internal class HasReadWriteOnlyProperty
    {
        public static Guid ReadOnly { get; }
        public static Guid WriteOnly { set { } }
    }
}
