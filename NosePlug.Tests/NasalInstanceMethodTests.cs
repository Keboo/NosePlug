using NosePlug.Tests.TestClasses;
using System.Reflection;
using Xunit;

namespace NosePlug.Tests;

public class NasalInstanceMethodTests
{
    [Fact]
    public async Task InstanceMethod_WithVoidMethod_CanAccessThis()
    {
        bool wasCalled = false;

        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethod())
            .Callback((HasInstanceMethods @this) =>
            {
                wasCalled = true;
                Assert.Equal(sut, @this);
            });

        using var _ = await Nasal.ApplyAsync(methodPlug);

        sut.VoidMethod();

        Assert.True(wasCalled);
    }

    [Fact]
    public async Task InstanceMethod_WithVoidMethodParameter_CanAccessParameter()
    {
        bool wasCalled = false;
        string? parameter = null;
        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethodWithParameter(""))
            .Callback((HasInstanceMethods @this, string p0) =>
            {
                wasCalled = true;
                parameter = p0;
                Assert.Equal(sut, @this);
            });

        using var _ = await Nasal.ApplyAsync(methodPlug);

        sut.VoidMethodWithParameter("foo");

        Assert.True(wasCalled);
        Assert.Equal("foo", parameter);
    }

    [Fact]
    public async Task InstanceMethod_WithIncorrectThisParameter_ThrowsExpectedException()
    {
        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethod())
            .Callback((string _) => { });

        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasInstanceMethods.VoidMethod has parameters (System.String) that do not match original method parameters (NosePlug.Tests.TestClasses.HasInstanceMethods this)", ex.Message);
    }

    [Fact]
    public async Task InstanceMethod_WithIncorrectThisParameterAndParameter_ThrowsExpectedException()
    {
        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethodWithParameter(""))
            .Callback((string _, string _) => { });

        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasInstanceMethods.VoidMethodWithParameter has parameters (System.String, System.String) that do not match original method parameters (NosePlug.Tests.TestClasses.HasInstanceMethods this, System.String)", ex.Message);
    }

    [Fact]
    public async Task InstanceMethod_WithPrivateMethod_CanBePlugged()
    {
        int invocationCount = 0;

        HasInstanceMethods sut = new();

        IInstanceMethodPlug noParamsPlug = Nasal.InstanceMethod<HasInstanceMethods>("NoParameters")
            .Callback((HasInstanceMethods _) => invocationCount++);

        using IDisposable _ = await Nasal.ApplyAsync(noParamsPlug);

        sut.InvokeNoParameters();

        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task InstanceMethod_WithReturnValue_CanBePlugged()
    {
        HasInstanceMethods sut = new();

        IInstanceMethodPlug<int> getIntValuePlug =
            Nasal.InstanceMethod((HasInstanceMethods x) => x.GetIntegerValue())
            .Returns((HasInstanceMethods _) => 2);

        using IDisposable _ = await Nasal.ApplyAsync(getIntValuePlug);

        Assert.Equal(2, sut.GetIntegerValue());
    }

    [Fact]
    public async Task InstanceMethod_WithBaseParameterType_CanBePlugged()
    {
        HasInstanceMethods sut = new();

        TestService service = new();
        IService? passedService = null;

        IInstanceMethodPlug getIntValuePlug =
            Nasal.InstanceMethod((HasInstanceMethods x) => x.HasParameter(null!))
            .Callback((HasInstanceMethods _, IService service) =>
            {
                passedService = service;
            });

        using IDisposable _ = await Nasal.ApplyAsync(getIntValuePlug);

        sut.HasParameter(service);

        Assert.Equal(service, passedService);
    }

    [Fact]
    public async Task InstanceMethod_WithBaseDefinedType_CanBePlugged()
    {
        HasInstanceMethods sut = new();

        TestService service = new();
        IService? passedService = null;

        IInstanceMethodPlug getIntValuePlug =
            Nasal.InstanceMethod((HasInstanceMethods x) => x.HasParameter(null!))
            .Callback((HasInstanceMethods _, IService service) =>
            {
                passedService = service;
            });

        using IDisposable _ = await Nasal.ApplyAsync(getIntValuePlug);

        sut.HasParameter(service);

        Assert.Equal(service, passedService);
    }

    [Fact]
    public async Task InstanceMethod_WithPrivateTypeParameter_CanBePlugged()
    {
        HasInstanceMethods sut = new();

        object? passedValue = null;
        Type? parameterType = typeof(HasInstanceMethods).GetNestedType("HiddenType", BindingFlags.NonPublic);
        Assert.NotNull(parameterType);

        IInstanceMethodPlug<string> getIntValuePlug =
            Nasal.InstanceMethod<HasInstanceMethods, string>("HasHiddenType", parameterType)
            .Returns((HasInstanceMethods @this, object hiddenType) =>
            {
                passedValue = hiddenType;
                return "foo";
            });

        using IDisposable _ = await Nasal.ApplyAsync(getIntValuePlug);

        string result = sut.InvokeHasHiddenType();
        Assert.Equal("foo", result);
        Assert.Equal(parameterType, passedValue?.GetType());
    }

    [Fact]
    public async Task InstanceMethod_WithBaseClassForThisParameter_CanBePlugged()
    {
        bool wasCalled = false;

        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethod())
            .Callback((object @this) =>
            {
                wasCalled = true;
                Assert.Equal(sut, @this);
            });

        using var _ = await Nasal.ApplyAsync(methodPlug);

        sut.VoidMethod();

        Assert.True(wasCalled);
    }
}