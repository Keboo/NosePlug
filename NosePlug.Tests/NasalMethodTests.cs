using System;
using System.Threading.Tasks;
using NosePlug.Tests.TestClasses;
using Xunit;

namespace NosePlug.Tests
{

    public class NasalMethodTests
    {
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

        [Fact(Skip = "TODO")]
        public void CanReplaceLinqWhereMethod()
        {

        }

        [Fact]
        public async Task CanReplacePublicMethod()
        {
            Nasal mocker = new();
            int invocationCount = 0;

            mocker.Method(() => HasPublicMethod.NoParameters())
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicMethod.NoParameters();

            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicMethodWithReturnValue()
        {
            Nasal mocker = new();

            mocker.Method(() => HasPublicMethod.ReturnValue())
                .Returns(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, HasPublicMethod.ReturnValue());
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithActionCallback()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethod())
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPublicMethod.AsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithAsyncCallback()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethod())
                .Callback(async () =>
                {
                    await Task.Yield();
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPublicMethod.AsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithReturnValueDelegateCallback()
        {
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethodWithReturn())
                .Returns(() => Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithReturnValueCallback()
        {
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethodWithReturn())
                .Returns(Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithTaskReturnValueCallback()
        {
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethodWithReturn())
                .Returns(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePublicMethodWithOverride()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.Overloaded(0))
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicMethod.Overloaded(42);
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicMethodWithOverrideWithParametersInCallback()
        {
            int invocationCount = 0;
            string? passedString = null;
            int passedValue = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.Overloaded("", 0))
                .Callback((string foo, int value) => {
                    passedString = foo;
                    passedValue = value;
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicMethod.Overloaded("Foo", 42);
            Assert.Equal(1, invocationCount);
            Assert.Equal("Foo", passedString);
            Assert.Equal(42, passedValue);
        }

        [Fact]
        public async Task CanReplacePublicGenericMethods()
        {
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.GenericMethod<int>())
                .Returns(42);

            using IDisposable _ = await mocker.ApplyAsync();

            int value = HasPublicMethod.GenericMethod<int>();
            string? stringValue = HasPublicMethod.GenericMethod<string>();

            Assert.Equal(42, value);
            Assert.Null(stringValue);
        }


        [Fact]
        public async Task CanReplacePrivateMethod()
        {
            Nasal mocker = new();
            int invocationCount = 0;

            mocker.Method<HasPrivateMethod>("NoParameters")
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateMethod.InvokeNoParameters();

            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateMethodWithReturnValue()
        {
            Nasal mocker = new();

            mocker.Method<HasPrivateMethod>("ReturnValue")
                .Returns(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, HasPrivateMethod.InvokeReturnValue());
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithActionCallback()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethod")
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPrivateMethod.InvokeAsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithAsyncCallback()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethod")
                .Callback(async () =>
                {
                    await Task.Yield();
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPrivateMethod.InvokeAsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithReturnValueDelegateCallback()
        {
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethodWithReturn")
                .Returns(() => Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithReturnValueCallback()
        {
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethodWithReturn")
                .Returns(Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithTaskReturnValueCallback()
        {
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethodWithReturn")
                .Returns(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePrivateMethodWithOverride()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("Overloaded", typeof(int))
                .Callback(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateMethod.InvokeOverloaded(42);
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateMethodWithOverrideWithParametersInCallback()
        {
            int invocationCount = 0;
            string? passedString = null;
            int passedValue = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("InvokeOverloaded", typeof(string), typeof(int))
                .Callback((string foo, int value) => {
                    passedString = foo;
                    passedValue = value;
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateMethod.InvokeOverloaded("Foo", 42);
            Assert.Equal(1, invocationCount);
            Assert.Equal("Foo", passedString);
            Assert.Equal(42, passedValue);
        }

        [Fact]
        public async Task CanReplacePrivateGenericMethods()
        {
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("GenericMethod", new Type[] { typeof(int) }, Array.Empty<Type>())
                .Returns(42);

            using IDisposable _ = await mocker.ApplyAsync();

            int value = HasPrivateMethod.InvokeGenericMethod<int>();
            string? stringValue = HasPrivateMethod.InvokeGenericMethod<string>();

            Assert.Equal(42, value);
            Assert.Null(stringValue);
        }
    }
}
