using NosePlug.Tests.TestClasses;
using System;
using System.Threading.Tasks;
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
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasInstanceMethods.VoidMethod has callback parameters (System.String) that do not match original method parameters (NosePlug.Tests.TestClasses.HasInstanceMethods this)", ex.Message);
    }

    [Fact]
    public async Task InstanceMethod_WithIncorrectThisParameterAndParameter_ThrowsExpectedException()
    {
        HasInstanceMethods sut = new();

        IInstanceMethodPlug methodPlug = Nasal.InstanceMethod<HasInstanceMethods>(x => x.VoidMethodWithParameter(""))
            .Callback((string _, string _) => { });

        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasInstanceMethods.VoidMethodWithParameter has callback parameters (System.String, System.String) that do not match original method parameters (NosePlug.Tests.TestClasses.HasInstanceMethods this, System.String)", ex.Message);
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
    public async Task CanDoHorribleHorribleThings()
    {
        HasInstanceMethods sut = new();

        //IInstanceMethodPlug noParamsPlug = Nasal.InstanceMethod<string>(s => s.ToString())
        //    .Returns((string _) => "someone stop me");

        //using IDisposable _ = await Nasal.ApplyAsync(noParamsPlug);

        Assert.Equal("someone stop me", "".ToString());
    }
}