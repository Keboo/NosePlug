﻿using Moq.AutoMock;
using NosePlug.Tests.TestClasses;
using Xunit;

namespace NosePlug.Tests
{
    public class AutoMockerTests
    {
        [Fact]
        public async Task CanMockStaticProperty()
        {
            Guid testGuid = Guid.NewGuid();

            AutoMocker mocker = new();
            mocker.StaticProperty(() => HasPublicProperty.Foo)
                  .Returns(testGuid);

            using IDisposable _ = await mocker.ApplyStaticMocksAsync();

            Assert.Equal(testGuid, HasPublicProperty.Foo);
        }
    }
}
