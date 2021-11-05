using NosePlug.Tests.TestClasses;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NosePlug.Tests
{
    public class NasalTests
    {
        [Fact]
        public async Task CanReplacePublicPropertyGetter()
        {
            Guid testGuid = Guid.NewGuid();
            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .Returns(() => testGuid);

            using IDisposable _ = await mocker.ApplyAsync();
            
            Assert.Equal(testGuid, HasPublicProperty.Foo);
        }

        [Fact(Skip = "TODO")]
        public async Task CanReplacePublicPropertySetter()
        {

        }

        [Fact]
        public async Task GetReplacePrivarPropertyGetter()
        {
            Guid testGuid = Guid.NewGuid();
            Nasal mocker = new();
            mocker.Property<HasPrivateProperty>("Foo")
                  .Returns(() => testGuid);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(testGuid, HasPrivateProperty.ReadPrivateProperty());
        }

        [Fact(Skip = "TODO")]
        public async Task CanReplacePrivatePropertySetter()
        {

        }

        [Fact]
        public async Task CanReplaceAndUndoDateTimeNow()
        {
            DateTime today = DateTime.Today;

            Nasal mocker = new();
            mocker.Property<DateTime>(nameof(DateTime.Today))
                  .Returns(() => new DateTime(1987, 4, 20));

            using (await mocker.ApplyAsync())
            {
                Assert.Equal(new DateTime(1987, 4, 20), DateTime.Today);
            }

            Assert.NotEqual(new DateTime(1987, 4, 20), DateTime.Today);
            Assert.Equal(today, DateTime.Today);
        }

        [Fact]
        public async Task CanReplaceAndUndoTaskRun()
        {
            Nasal mocker = new();
            mocker.Method(() => Task.Run((Func<int>)null!))
                  .Returns(() => Task.FromResult(42));

            using (await mocker.ApplyAsync())
            {
                Assert.Equal(42, await Task.Run(() => 21));
            }

            Assert.Equal(21, await Task.Run(() => 21));
        }
    }
}
