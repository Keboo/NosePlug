using System;
using Xunit;

namespace NosePlug.Tests
{
    public class NasalMethodPlugExtensionsTests
    {
        [Fact]
        public void ReturnsValue_WithNullMethodPlug_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalMethodPlugExtensions.ReturnsValue((INasalMethodPlug)null!, 0));
            Assert.Equal("plug", ex.ParamName);
        }

        [Fact]
        public void ReturnsValue_WithNullPropertyPlug_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => NasalMethodPlugExtensions.ReturnsValue((INasalPropertyPlug<int>)null!, 0));
            Assert.Equal("plug", ex.ParamName);
        }
    }
}
