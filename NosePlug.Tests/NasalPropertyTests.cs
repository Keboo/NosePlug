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
            DateTime now = DateTime.Now;

            Nasal mocker = new();
            mocker.Property(() => DateTime.Now)
                  .Returns(() => new DateTime(1987, 4, 20));

            using (await mocker.ApplyAsync())
            {
                Assert.Equal(new DateTime(1987, 4, 20), DateTime.Now);
            }

            Assert.NotEqual(new DateTime(1987, 4, 20), DateTime.Now);
            Assert.Equal(now.Date, DateTime.Now.Date);
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
                  .ReturnsValue(returnValue)
                  .ReplaceSetter(x => passedGuid = x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicProperty.Foo = setValue;
            Guid receivedValue = HasPublicProperty.Foo;

            Assert.Equal(setValue, passedGuid);
            Assert.Equal(returnValue, receivedValue);
        }

        [Fact]
        public async Task CanCallReturnsMultipleTimes()
        {
            Guid firstValue = Guid.NewGuid();
            Guid secondValue = Guid.NewGuid();
            Guid thirdValue = Guid.NewGuid();

            Nasal mocker = new();
            mocker.Property(() => HasPublicProperty.Foo)
                  .ReturnsValue(firstValue)
                  .ReturnsValue(secondValue)
                  .ReturnsValue(thirdValue);

            using IDisposable _ = await mocker.ApplyAsync();

            Guid receivedValue = HasPublicProperty.Foo;

            Assert.Equal(thirdValue, receivedValue);
        }

        [Fact]
        public async Task CanCallReplaceSetterMultipleTimes()
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
        public async Task CanCallOriginalProperty()
        {
            Nasal mocker = new();
            mocker.Property(() => HasFullProperty.Property)
                  .CallOriginal();

            using IDisposable _ = await mocker.ApplyAsync();

            HasFullProperty.Property = 42;

            Assert.Equal(42, HasFullProperty._field);
        }

        [Fact]
        public async Task CanOverridePropertyGetterWithPropertyInfo()
        {
            var propertyInfo = typeof(HasPublicProperty).GetProperty(nameof(HasPublicProperty.Foo))!;
            Guid testGuid = Guid.NewGuid();
            Nasal mocker = new();
            mocker.Property(propertyInfo)
                  .Returns(() => testGuid);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(testGuid, HasPublicProperty.Foo);
        }

        [Fact]
        public async Task CanOverridePropertySetterWithPropertyInfo()
        {
            var propertyInfo = typeof(HasPublicProperty).GetProperty(nameof(HasPublicProperty.Foo))!;
            Guid testGuid = Guid.NewGuid();
            Guid passedGuid = Guid.Empty;

            Nasal mocker = new();
            mocker.Property(propertyInfo)
                  .ReplaceSetter(x => passedGuid = (Guid)x);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicProperty.Foo = testGuid;

            Assert.Equal(testGuid, passedGuid);
            Assert.NotEqual(testGuid, HasPublicProperty.Foo);
        }
    }
}
