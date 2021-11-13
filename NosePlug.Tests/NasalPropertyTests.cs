using HarmonyLib;
using NosePlug.Tests.TestClasses;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            HarmonyLib.Harmony.DEBUG = true;
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

        
        public static Guid Foo { get; set; }

        static Guid testValue;
        [Fact]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void TestHarmony()
        {
            Guid setValue = Guid.NewGuid();

            var prop = GetType().GetProperty("Foo");
            var prefix = GetType().GetMethod(nameof(SetterPrefix));

            var harmony = new Harmony("abc");
            var processor = harmony.CreateProcessor(prop.SetMethod);
            processor.AddPrefix(prefix);
            processor.Patch();

            Foo = setValue;

            Assert.Equal(setValue, testValue);
        }

        public static bool SetterPrefix(Guid __0)
        {
            testValue = __0;
            return true;
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
    }
}
