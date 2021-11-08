﻿using System;

namespace NosePlug.Tests.TestClasses
{
    internal class HasPrivateProperty
    {
        private static Guid Foo { get; set; }

        public static Guid ReadPrivateProperty() => Foo;
        public static Guid WritePrivateProperty(Guid value) => Foo = value;
    }
}