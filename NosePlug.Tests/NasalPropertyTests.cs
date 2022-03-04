using System.Linq.Expressions;
using NosePlug.Tests.TestClasses;
using Xunit;

namespace NosePlug.Tests;

public class NasalPropertyTests
{

    [Fact]
    public async Task CanReplacePublicPropertyGetter()
    {
        Guid testGuid = Guid.NewGuid();
        var fooPlug = Nasal.Property(() => HasPublicProperty.Foo)
                .Returns(() => testGuid);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        Assert.Equal(testGuid, HasPublicProperty.Foo);
    }

    [Fact]
    public async Task CanReplacePublicPropertySetter()
    {
        Guid testGuid = Guid.NewGuid();
        Guid passedGuid = Guid.Empty;

        var fooPlug = Nasal.Property(() => HasPublicProperty.Foo)
              .Callback(x => passedGuid = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        HasPublicProperty.Foo = testGuid;

        Assert.Equal(testGuid, passedGuid);
        Assert.NotEqual(testGuid, HasPublicProperty.Foo);
    }

    [Fact]
    public async Task CanReplacePrivatePropertyGetter()
    {
        Guid testGuid = Guid.NewGuid();
        var fooPlug = Nasal.Property<HasPrivateProperty, Guid>("Foo")
              .Returns(() => testGuid);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        Assert.Equal(testGuid, HasPrivateProperty.ReadPrivateProperty());
    }

    [Fact]
    public async Task CanReplacePrivatePropertySetter()
    {
        Guid testGuid = Guid.NewGuid();
        Guid passedGuid = Guid.Empty;

        var fooPlug = Nasal.Property<HasPrivateProperty, Guid>("Foo")
              .Callback(x => passedGuid = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        HasPrivateProperty.WritePrivateProperty(testGuid);

        Assert.Equal(testGuid, passedGuid);
        Assert.NotEqual(testGuid, HasPublicProperty.Foo);
    }

    //[Fact]
    //public async Task CanReplaceAndUndoDateTimeNow()
    //{
    //    DateTime now = DateTime.Now;

    //    var nowPlug = Nasal.Property(() => DateTime.Now)
    //          .Returns(() => new DateTime(1987, 4, 20));

    //    using (await Nasal.ApplyAsync(nowPlug))
    //    {
    //        Assert.Equal(new DateTime(1987, 4, 20), DateTime.Now);
    //    }

    //    Assert.NotEqual(new DateTime(1987, 4, 20), DateTime.Now);
    //    Assert.Equal(now.Date, DateTime.Now.Date);
    //}

    [Fact]
    public async Task CanReplacePrivateSetter()
    {
        Guid passedGuid = Guid.Empty;
        Guid setValue = Guid.NewGuid();

        var fooPlug = Nasal.Property(() => HasPrivateSetter.Foo)
              .Callback(x => passedGuid = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        HasPrivateSetter.SetProperty(setValue);

        Assert.Equal(setValue, passedGuid);
    }

    [Fact]
    public async Task CanReplaceBothGetterAndSetter()
    {
        Guid passedGuid = Guid.Empty;
        Guid returnValue = Guid.NewGuid();
        Guid setValue = Guid.NewGuid();

        var fooPlug = Nasal.Property(() => HasPublicProperty.Foo)
              .Returns(returnValue)
              .Callback(x => passedGuid = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

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

        var fooPlug = Nasal.Property(() => HasPublicProperty.Foo)
              .Returns(firstValue)
              .Returns(secondValue)
              .Returns(thirdValue);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        Guid receivedValue = HasPublicProperty.Foo;

        Assert.Equal(thirdValue, receivedValue);
    }

    [Fact]
    public async Task CanCallCallbackMultipleTimes()
    {
        Guid firstValue = Guid.Empty;
        Guid secondValue = Guid.Empty;
        Guid setValue = Guid.NewGuid();

        var fooPlug = Nasal.Property(() => HasPublicProperty.Foo)
              .Callback(x => firstValue = x)
              .Callback(x => secondValue = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        HasPublicProperty.Foo = setValue;

        Assert.Equal(Guid.Empty, firstValue);
        Assert.Equal(setValue, secondValue);
    }

    [Fact]
    public void WhenPropertyIsReadOnly_CallingCallbackErrors()
    {
        Assert.Throws<NasalException>(() =>
            Nasal.Property(() => HasReadWriteOnlyProperty.ReadOnly)
                  .Callback(_ => { })
        );
    }

    [Fact]
    public void WhenPropertyIsWriteOnly_CallingReturnsErrors()
    {
        Assert.Throws<NasalException>(() =>
            Nasal.Property<HasReadWriteOnlyProperty, Guid>(nameof(HasReadWriteOnlyProperty.WriteOnly))
                  .Returns(() => Guid.NewGuid())
        );
    }

    [Fact]
    public async Task CanCallOriginalProperty()
    {
        var propertyPlug = Nasal.Property(() => HasFullProperty.Property)
              .CallOriginal();

        using IDisposable _ = await Nasal.ApplyAsync(propertyPlug);

        HasFullProperty.Property = 42;

        Assert.Equal(42, HasFullProperty._field);
    }

    [Fact]
    public async Task CanOverridePropertyGetterWithPropertyInfo()
    {
        var propertyInfo = typeof(HasPublicProperty).GetProperty(nameof(HasPublicProperty.Foo))!;
        Guid testGuid = Guid.NewGuid();
        var fooPlug = Nasal.Property<Guid>(propertyInfo)
              .Returns(() => testGuid);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        Assert.Equal(testGuid, HasPublicProperty.Foo);
    }

    [Fact]
    public async Task CanOverridePropertySetterWithPropertyInfo()
    {
        var propertyInfo = typeof(HasPublicProperty).GetProperty(nameof(HasPublicProperty.Foo))!;
        Guid testGuid = Guid.NewGuid();
        Guid passedGuid = Guid.Empty;

        var fooPlug = Nasal.Property<Guid>(propertyInfo)
              .Callback(x => passedGuid = x);

        using IDisposable _ = await Nasal.ApplyAsync(fooPlug);

        HasPublicProperty.Foo = testGuid;

        Assert.Equal(testGuid, passedGuid);
        Assert.NotEqual(testGuid, HasPublicProperty.Foo);
    }

    [Fact]
    public void Property_WhenNameIsNull_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => Nasal.Property<HasPublicProperty, Guid>(null!));
    }

    [Fact]
    public void Property_WhenPropertyNameIsNotFound_ThrowsException()
    {
        var ex = Assert.Throws<MissingMemberException>(() => Nasal.Property<HasPublicProperty, Guid>("DoesNotExist"));
        Assert.Contains("Could not find property 'DoesNotExist'", ex.Message);
    }

    [Fact]
    public void PropertyExpression_WhenExpressionIsNull_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Nasal.Property((Expression<Func<Guid>>)null!));
        Assert.Equal("propertyExpression", ex.ParamName);
    }

    [Fact]
    public void PropertyExpression_WhenExpressionIsNotAPropertyExpression_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Nasal.Property(() => Guid.NewGuid()));
        Assert.Equal("Expresion is not a member expression to a property", ex.Message);
    }
}
