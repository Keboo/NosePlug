using System.Linq.Expressions;
using NosePlug.Tests.TestClasses;
using Xunit;

namespace NosePlug.Tests;

public class NasalStaticMethodTests
{

    [Fact]
    public async Task CanReplaceAndUndoTaskRun()
    {
        var taskRunPlug = Nasal.Method(() => Task.Run((Func<int>)null!))
              .Returns(() => Task.FromResult(42));

        using (await Nasal.ApplyAsync(taskRunPlug))
        {
            Assert.Equal(42, await Task.Run(() => 21));
        }

        Assert.Equal(21, await Task.Run(() => 21));
    }

    [Fact]
    public async Task CanReplaceLinqWhereMethod()
    {
        var enumerablePlug = Nasal.Method(() => Enumerable.Where(Enumerable.Empty<int>(), x => true))
              .Returns(new[] { 1, 2, 3 });

        using IDisposable _ = await Nasal.ApplyAsync(enumerablePlug);

        var rv = Enumerable.Range(1, 10).Where(x => x > 5).ToArray();

        Assert.Equal(new[] { 1, 2, 3 }, rv);
    }

    [Fact]
    public async Task CanReplacePublicMethod()
    {
        int invocationCount = 0;

        var noParamsPlug = Nasal.Method(() => HasPublicMethod.NoParameters())
            .Callback(() => invocationCount++);

        using IDisposable _ = await Nasal.ApplyAsync(noParamsPlug);

        HasPublicMethod.NoParameters();

        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePublicMethodWithReturnValue()
    {
        var returnValuePlug = Nasal.Method(() => HasPublicMethod.ReturnValue())
            .CallOriginal()
            .Returns(4);

        using IDisposable _ = await Nasal.ApplyAsync(returnValuePlug);

        Assert.Equal(4, HasPublicMethod.ReturnValue());
    }

    [Fact]
    public async Task CanReplacePublicAsyncMethodWithAsyncCallback()
    {
        int invocationCount = 0;

        var asyncPlug = Nasal.Method(() => HasPublicMethod.AsyncMethod())
            .Returns(async () =>
            {
                await Task.Yield();
                invocationCount++;
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        await HasPublicMethod.AsyncMethod();
        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePublicAsyncMethodInludingParametersWithActionCallback()
    {
        int invocationCount = 0;
        int passedValue = 0;
        var asyncPlug = Nasal.Method(() => HasPublicMethod.AsyncMethod(0))
            .Returns((int value) =>
            {
                invocationCount++;
                passedValue = value;
                return Task.CompletedTask;
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        await HasPublicMethod.AsyncMethod(42);
        Assert.Equal(1, invocationCount);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePublicAsyncMethodInludingMultipleParametersWithActionCallback()
    {
        int invocationCount = 0;
        string passString = "";
        int passedValue = 0;
        var asyncPlug = Nasal.Method(() => HasPublicMethod.AsyncMethod("", 0))
            .Returns((string @string, int value) =>
            {
                invocationCount++;
                passString = @string;
                passedValue = value;
                return Task.FromResult(0.0);
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        await HasPublicMethod.AsyncMethod("Test", 42);
        Assert.Equal(1, invocationCount);
        Assert.Equal("Test", passString);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePublicAsyncMethodWithReturnValueDelegateCallback()
    {
        var asyncPlug = Nasal.Method(() => HasPublicMethod.AsyncMethodWithReturn())
            .Returns(() => Task.FromResult(4));

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
    }

    [Fact]
    public async Task CanReplacePublicAsyncMethodWithReturnValueCallback()
    {
        var asyncPlug = Nasal.Method(() => HasPublicMethod.AsyncMethodWithReturn())
            .Returns(Task.FromResult(4));

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
    }

    [Fact]
    public async Task CanReplacePublicMethodWithOverride()
    {
        int invocationCount = 0;
        var overloadedPlug = Nasal.Method(() => HasPublicMethod.Overloaded(0))
            .Callback(() => invocationCount++);

        using IDisposable _ = await Nasal.ApplyAsync(overloadedPlug);

        HasPublicMethod.Overloaded(42);
        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePublicMethodWithOverrideWithParametersInCallback()
    {
        int invocationCount = 0;
        string? passedString = null;
        int passedValue = 0;
        var overloadedPlug = Nasal.Method(() => HasPublicMethod.Overloaded("", 0))
            .Callback((string first, int second) =>
            {
                passedString = first;
                passedValue = second;
                invocationCount++;
            });

        using IDisposable _ = await Nasal.ApplyAsync(overloadedPlug);

        HasPublicMethod.Overloaded("Foo", 42);
        Assert.Equal(1, invocationCount);
        Assert.Equal("Foo", passedString);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePublicGenericMethods()
    {
        var genericPlug = Nasal.Method(() => HasPublicMethod.GenericMethod<int>())
            .Returns(42);

        using IDisposable _ = await Nasal.ApplyAsync(genericPlug);

        int value = HasPublicMethod.GenericMethod<int>();
        string? stringValue = HasPublicMethod.GenericMethod<string>();

        Assert.Equal(42, value);
        Assert.Null(stringValue);
    }

    [Fact]
    public async Task CanReplacePrivateMethod()
    {
        int invocationCount = 0;

        var noParamsPlug = Nasal.Method<HasPrivateMethod>("NoParameters")
            .Callback(() => invocationCount++);

        using IDisposable _ = await Nasal.ApplyAsync(noParamsPlug);

        HasPrivateMethod.InvokeNoParameters();

        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePrivateMethodWithReturnValue()
    {
        var returnPlug = Nasal.Method<HasPrivateMethod, int>("ReturnValue")
            .Returns(4);

        using IDisposable _ = await Nasal.ApplyAsync(returnPlug);

        Assert.Equal(4, HasPrivateMethod.InvokeReturnValue());
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodWithActionCallback()
    {
        int invocationCount = 0;
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task>("AsyncMethod")
            .Returns(() =>
            {
                invocationCount++;
                return Task.CompletedTask;
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        await HasPrivateMethod.InvokeAsyncMethod();
        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodWithAsyncCallback()
    {
        int invocationCount = 0;
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task>("AsyncMethod")
            .Returns(async () =>
            {
                await Task.Yield();
                invocationCount++;
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug
            );

        await HasPrivateMethod.InvokeAsyncMethod();
        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodInludingParametersWithActionCallback()
    {
        int invocationCount = 0;
        int passedValue = 0;
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task>("AsyncMethod", typeof(int))
            .Returns((int value) =>
            {
                invocationCount++;
                passedValue = value;
                return Task.CompletedTask;
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        await HasPrivateMethod.InvokeAsyncMethod(42);
        Assert.Equal(1, invocationCount);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodInludingMultipleParametersWithActionCallback()
    {
        int invocationCount = 0;
        string passString = "";
        int passedValue = 0;
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task<double>>("AsyncMethod", typeof(string), typeof(int))
            .Returns((string @string, int value) =>
            {
                invocationCount++;
                passString = @string;
                passedValue = value;
                return Task.FromResult(2.3);
            });

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        double rv = await HasPrivateMethod.InvokeAsyncMethod("Test", 42);
        Assert.Equal(2.3, rv);
        Assert.Equal(1, invocationCount);
        Assert.Equal("Test", passString);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodWithReturnValueDelegateCallback()
    {
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task<int>>("AsyncMethodWithReturn")
            .Returns(() => Task.FromResult(4));

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
    }

    [Fact]
    public async Task CanReplacePrivateAsyncMethodWithReturnValueCallback()
    {
        var asyncPlug = Nasal.Method<HasPrivateMethod, Task<int>>("AsyncMethodWithReturn")
            .Returns(Task.FromResult(4));

        using IDisposable _ = await Nasal.ApplyAsync(asyncPlug);

        Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
    }

    [Fact]
    public async Task CanReplacePrivateMethodWithOverride()
    {
        int invocationCount = 0;
        var overloadedPlug = Nasal.Method<HasPrivateMethod>("Overloaded", typeof(int))
            .Callback(() => invocationCount++);

        using IDisposable _ = await Nasal.ApplyAsync(overloadedPlug);

        HasPrivateMethod.InvokeOverloaded(42);
        Assert.Equal(1, invocationCount);
    }

    [Fact]
    public async Task CanReplacePrivateMethodWithOverrideWithParametersInCallback()
    {
        int invocationCount = 0;
        string? passedString = null;
        int passedValue = 0;
        var overloadedPlug = Nasal.Method<HasPrivateMethod>("Overloaded", typeof(string), typeof(int))
            .Callback((string foo, int value) =>
            {
                passedString = foo;
                passedValue = value;
                invocationCount++;
            });

        using IDisposable _ = await Nasal.ApplyAsync(overloadedPlug);

        HasPrivateMethod.InvokeOverloaded("Foo", 42);
        Assert.Equal(1, invocationCount);
        Assert.Equal("Foo", passedString);
        Assert.Equal(42, passedValue);
    }

    [Fact]
    public async Task CanReplacePrivateGenericMethods()
    {
        var genericPlug = Nasal.Method<HasPrivateMethod, int>("GenericMethod", new Type[] { typeof(int) }, Array.Empty<Type>())
            .Returns(42);

        using IDisposable _ = await Nasal.ApplyAsync(genericPlug);

        int value = HasPrivateMethod.InvokeGenericMethod<int>();
        string? stringValue = HasPrivateMethod.InvokeGenericMethod<string>();

        Assert.Equal(42, value);
        Assert.Null(stringValue);
    }

    [Fact]
    public async Task CanInvokeOriginalVoidReturningMethod()
    {
        int invocationCount = 0;
        var overloadedPlug = Nasal.Method(() => HasPublicMethod.Overloaded(0))
                .Callback(() => invocationCount++)
                .CallOriginal();

        using IDisposable _ = await Nasal.ApplyAsync(overloadedPlug);
        HasPublicMethod.OverloadedValue = 0;

        HasPublicMethod.Overloaded(42);

        Assert.Equal(1, invocationCount);
        Assert.Equal(42, HasPublicMethod.OverloadedValue);
    }

    [Fact]
    public async Task CanInvokeOriginalTypeReturningMethod()
    {
        int invocationCount = 0;
        var returnPlug = Nasal.Method(() => HasPublicMethod.ReturnValue())
              .Returns(() =>
              {
                  invocationCount++;
                  return 3;
              })
              .CallOriginal();

        using IDisposable _ = await Nasal.ApplyAsync(returnPlug);
        HasPublicMethod.ReturnValueCalled = false;

        int value = HasPublicMethod.ReturnValue();

        Assert.Equal(1, invocationCount);
        Assert.Equal(42, value);
        Assert.True(HasPublicMethod.ReturnValueCalled);
    }

    [Fact]
    public void MethodExpression_WhenExpressionIsNull_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Nasal.Method((Expression<Action>)null!));
        Assert.Equal("methodExpression", ex.ParamName);
    }

    [Fact]
    public void MethodExpressionGeneric_WhenExpressionIsNull_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Nasal.Method((Expression<Func<int>>)null!));
        Assert.Equal("methodExpression", ex.ParamName);
    }

    [Fact]
    public void MethodExpressionGeneric_WhenExpressionIsNotAMethodCall_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Nasal.Method(() => HasPublicProperty.Foo));
        Assert.Equal("methodExpression", ex.ParamName);
        Assert.Contains("Expresion is not a method call expression", ex.Message);
    }

    [Fact]
    public void MethodGeneric_WhenMethodNameIsNull_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() => Nasal.Method<HasPublicMethod>((string)null!));
        Assert.Equal("methodName", ex.ParamName);
    }

    [Fact]
    public void MethodGeneric_WhenMethodNameIsNotFound_ThrowsException()
    {
        var ex = Assert.Throws<MissingMethodException>(() => Nasal.Method<HasPublicMethod>("DoesNotExist"));
        Assert.Contains("HasPublicMethod", ex.Message);
        Assert.Contains("DoesNotExist", ex.Message);
    }

    [Fact]
    public async Task Method_WrongNumberOfParameters_ThrowsException()
    {
        IMethodPlug<int> methodPlug = Nasal.Method(() => HasPublicMethod.ReturnValue())
            .Returns((string _, int _) => 42);
        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasPublicMethod.ReturnValue has parameters (System.String, System.Int32) that do not match original method parameters (<empty>)", ex.Message);
    }

    [Fact]
    public async Task Method_WrongReturnValue_ThrowsException()
    {
        IMethodPlug<string> methodPlug = Nasal.Method<HasPublicMethod, string>(nameof(HasPublicMethod.ReturnValue))
            .Returns(() => "42");
        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasPublicMethod.ReturnValue has return type (System.String) that do not match original method return type (System.Int32)", ex.Message);
    }

    [Fact]
    public async Task Method_WrongParameterTypes_ThrowsException()
    {
        IMethodPlug methodPlug = Nasal.Method(() => HasPublicMethod.Overloaded("", 0))
            .Callback((int _, string _) => { });
        var ex = await Assert.ThrowsAsync<NasalException>(() => Nasal.ApplyAsync(methodPlug));
        Assert.Equal("Plug for NosePlug.Tests.TestClasses.HasPublicMethod.Overloaded has parameters (System.Int32, System.String) that do not match original method parameters (System.String, System.Int32)", ex.Message);
    }

    [Fact]
    public async Task Method_CallbackWithInterfaceForParameterTypes_ReceivedCallback()
    {
        IService? service = null;
        IMethodPlug methodPlug = Nasal.Method(() => HasPublicMethod.HasServiceParameter(null!))
            .Callback((IService s) => service = s);
        using var _ = await Nasal.ApplyAsync(methodPlug);

        TestService expected = new();
        HasPublicMethod.HasServiceParameter(expected);

        Assert.Equal(expected, service);
    }

    [Fact]
    public async Task Method_InterfaceReturnValue_ReturnsValue()
    {
        IService expected = new TestService();
        IMethodPlug<IService> methodPlug = Nasal.Method<IService>(() => HasPublicMethod.HasServiceReturnValue())
            .Returns(() => expected);
        using var _ = await Nasal.ApplyAsync(methodPlug);

        TestService actual = HasPublicMethod.HasServiceReturnValue();
        Assert.Equal(expected, actual);
    }
}
