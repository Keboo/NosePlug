using System;
using Xunit;

namespace NosePlug.Tests
{
    public class PlugExtensionsTests
    {
        [Fact]
        public void Returns_WithNullMethodPlug_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => PlugExtensions.Returns((IMethodPlug<int>)null!, 0));
            Assert.Equal("plug", ex.ParamName);
        }

        [Fact]
        public void Returns_WithNullPropertyPlug_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => PlugExtensions.Returns((IPropertyPlug<int>)null!, 0));
            Assert.Equal("plug", ex.ParamName);
        }
    }
}
