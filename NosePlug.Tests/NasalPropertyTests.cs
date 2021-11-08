using NosePlug.Tests.TestClasses;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NosePlug.Tests
{
    public class NasalPropertyTests
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

        [Fact]
        public async Task CanReplacePublicPropertySetter()
        {
            Guid testGuid = Guid.NewGuid();
            Guid passedGuid = Guid.Empty;

            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .ReplaceSetter(x => passedGuid = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicProperty.Foo = testGuid;

            Assert.Equal(testGuid, passedGuid);
            Assert.NotEqual(testGuid, HasPublicProperty.Foo);
        }

        [Fact]
        public async Task GetReplacePrivarPropertyGetter()
        {
            Guid testGuid = Guid.NewGuid();
            Nasal mocker = new();
            mocker.Property<HasPrivateProperty, Guid>("Foo")
                  .Returns(() => testGuid);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(testGuid, HasPrivateProperty.ReadPrivateProperty());
        }

        [Fact]
        public async Task CanReplacePrivatePropertySetter()
        {
            Guid testGuid = Guid.NewGuid();
            Guid passedGuid = Guid.Empty;

            Nasal mocker = new();
            mocker.Property<HasPrivateProperty, Guid>("Foo")
                  .ReplaceSetter(x => passedGuid = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateProperty.WritePrivateProperty(testGuid);

            Assert.Equal(testGuid, passedGuid);
            Assert.NotEqual(testGuid, HasPublicProperty.Foo);
        }

        [Fact]
        public async Task CanReplaceAndUndoDateTimeNow()
        {
            DateTime today = DateTime.Today;

            Nasal mocker = new();
            mocker.Property(() => DateTime.Today)
                  .Returns(() => new DateTime(1987, 4, 20));

            using (await mocker.ApplyAsync())
            {
                Assert.Equal(new DateTime(1987, 4, 20), DateTime.Today);
            }

            Assert.NotEqual(new DateTime(1987, 4, 20), DateTime.Today);
            Assert.Equal(today, DateTime.Today);
        }

        [Fact]
        public async Task CanReplacePrivateSetter()
        {
            Guid passedGuid = Guid.Empty;
            Guid setValue = Guid.NewGuid();

            Nasal mocker = new();
            mocker.Property(() => HasPrivateSetter.Foo)
                  .ReplaceSetter(x => passedGuid = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateSetter.SetPropety(setValue);

            Assert.Equal(setValue, passedGuid);
        }

        [Fact]
        public async Task CanReplaceBothGetterAndSetter()
        {
            Guid passedGuid = Guid.Empty;
            Guid returnValue = Guid.NewGuid();
            Guid setValue = Guid.NewGuid();

            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .Returns(returnValue)
                  .ReplaceSetter(x => passedGuid = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicProperty.Foo = setValue;
            Guid receivedValue = HasPublicProperty.Foo;

            Assert.Equal(setValue, passedGuid);
            Assert.Equal(returnValue, receivedValue);
        }

        [Fact]
        public async Task CallCallReturnsMultipleTimes()
        {
            Guid firstValue = Guid.NewGuid();
            Guid secondValue = Guid.NewGuid();
            Guid thirdValue = Guid.NewGuid();

            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .Returns(firstValue)
                  .Returns(secondValue)
                  .Returns(thirdValue);

            using IDisposable _ = await mocker.ApplyAsync();

            Guid receivedValue = HasPublicProperty.Foo;

            Assert.Equal(thirdValue, receivedValue);
        }

        [Fact]
        public async Task CallCallReplaceSetterMultipleTimes()
        {
            Guid firstValue = Guid.Empty;
            Guid secondValue = Guid.Empty;
            Guid setValue = Guid.NewGuid();

            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .ReplaceSetter(x => firstValue = x)
                  .ReplaceSetter(x => secondValue = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicProperty.Foo = setValue;

            Assert.Equal(Guid.Empty, firstValue);
            Assert.Equal(setValue, secondValue);
        }

        [Fact]
        public void WhenPropertyIsReadOnly_CallingReplaceSetterErrors()
        {
            Nasal mocker = new();
            Assert.Throws<NasalException>(() => 
                mocker.Property(() => HasReadWriteOnlyProperty.ReadOnly)
                      .ReplaceSetter(_ => { })
            );
        }

        [Fact]
        public void WhenPropertyIsWriteOnly_CallingReturnsErrors()
        {
            Nasal mocker = new();
            Assert.Throws<NasalException>(() =>
                mocker.Property<HasReadWriteOnlyProperty, Guid>(nameof(HasReadWriteOnlyProperty.WriteOnly))
                      .Returns(() => Guid.NewGuid())
            );
        }

        [Fact]
        public async Task CanCallBaseMethod()
        {
            Nasal mocker = new();
            mocker.Property(() => HasFullProperty.Property)
                  .CallBase();

            using IDisposable _ = await mocker.ApplyAsync();

            HasFullProperty.Property = 42;

            Assert.Equal(42, HasFullProperty._field);
        }
    }
}
