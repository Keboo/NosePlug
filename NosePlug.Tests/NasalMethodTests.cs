using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public async Task CanReplaceLinqWhereMethod()
        {
            Nasal mocker = new();
            mocker.Method(() => Enumerable.Where(Enumerable.Empty<int>(), x => true))
                  .ReturnsValue<IEnumerable<int>>(new[] { 1, 2, 3 });

            using IDisposable _ = await mocker.ApplyAsync();

            var rv = Enumerable.Range(1, 10).Where(x => x > 5).ToArray();

            Assert.Equal(new[] { 1, 2, 3 }, rv);
        }

        [Fact]
        public async Task CanReplacePublicMethod()
        {
            Nasal mocker = new();
            int invocationCount = 0;

            mocker.Method(() => HasPublicMethod.NoParameters())
                .ReplaceWith(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPublicMethod.NoParameters();

            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicMethodWithReturnValue()
        {
            Nasal mocker = new();

            mocker.Method(() => HasPublicMethod.ReturnValue())
                .ReturnsValue(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, HasPublicMethod.ReturnValue());
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodWithActionReplaceWith()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethod())
                .ReplaceWith(() => invocationCount++);

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
                .Returns(async () =>
                {
                    await Task.Yield();
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPublicMethod.AsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePublicAsyncMethodInludingParametersWithActionCallback()
        {
            int invocationCount = 0;
            int passedValue = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethod(0))
                .ReplaceWith((int value) =>
                {
                    invocationCount++;
                    passedValue = value;
                });

            using IDisposable _ = await mocker.ApplyAsync();

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
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.AsyncMethod("", 0))
                .ReplaceWith((string @string, int value) =>
                {
                    invocationCount++;
                    passString = @string;
                    passedValue = value;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPublicMethod.AsyncMethod("Test", 42);
            Assert.Equal(1, invocationCount);
            Assert.Equal("Test", passString);
            Assert.Equal(42, passedValue);
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
                .ReturnsValue(Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPublicMethod.AsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePublicMethodWithOverride()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.Overloaded(0))
                .ReplaceWith(() => invocationCount++);

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
                .ReplaceWith((string first, int second) =>
                {
                    passedString = first;
                    passedValue = second;
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
                .ReturnsValue(42);

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
                .ReplaceWith(() => invocationCount++);

            using IDisposable _ = await mocker.ApplyAsync();

            HasPrivateMethod.InvokeNoParameters();

            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateMethodWithReturnValue()
        {
            Nasal mocker = new();

            mocker.Method<HasPrivateMethod>("ReturnValue")
                .ReturnsValue(4);

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, HasPrivateMethod.InvokeReturnValue());
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodWithActionCallback()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethod")
                .ReplaceWith(() => invocationCount++);

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
                .Returns(async () =>
                {
                    await Task.Yield();
                    invocationCount++;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPrivateMethod.InvokeAsyncMethod();
            Assert.Equal(1, invocationCount);
        }

        [Fact]
        public async Task CanReplacePrivateAsyncMethodInludingParametersWithActionCallback()
        {
            int invocationCount = 0;
            int passedValue = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethod", typeof(int))
                .ReplaceWith((int value) =>
                {
                    invocationCount++;
                    passedValue = value;
                });

            using IDisposable _ = await mocker.ApplyAsync();

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
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("AsyncMethod", typeof(string), typeof(int))
                .ReplaceWith((string @string, int value) =>
                {
                    invocationCount++;
                    passString = @string;
                    passedValue = value;
                });

            using IDisposable _ = await mocker.ApplyAsync();

            await HasPrivateMethod.InvokeAsyncMethod("Test", 42);
            Assert.Equal(1, invocationCount);
            Assert.Equal("Test", passString);
            Assert.Equal(42, passedValue);
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
                .ReturnsValue(Task.FromResult(4));

            using IDisposable _ = await mocker.ApplyAsync();

            Assert.Equal(4, await HasPrivateMethod.InvokeAsyncMethodWithReturn());
        }

        [Fact]
        public async Task CanReplacePrivateMethodWithOverride()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method<HasPrivateMethod>("Overloaded", typeof(int))
                .ReplaceWith(() => invocationCount++);

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
            mocker.Method<HasPrivateMethod>("Overloaded", typeof(string), typeof(int))
                .ReplaceWith((string foo, int value) =>
                {
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
                .ReturnsValue(42);

            using IDisposable _ = await mocker.ApplyAsync();

            int value = HasPrivateMethod.InvokeGenericMethod<int>();
            string? stringValue = HasPrivateMethod.InvokeGenericMethod<string>();

            Assert.Equal(42, value);
            Assert.Null(stringValue);
        }

        [Fact]
        public async Task CanInvokeOriginalVoidReturningMethod()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.Overloaded(0))
                    .ReplaceWith(() => invocationCount++)
                    .CallOriginal();

            using IDisposable _ = await mocker.ApplyAsync();
            HasPublicMethod.OverloadedValue = 0;

            HasPublicMethod.Overloaded(42);

            Assert.Equal(1, invocationCount);
            Assert.Equal(42, HasPublicMethod.OverloadedValue);
        }

        [Fact]
        public async Task CanInvokeOriginalTypeReturningMethod()
        {
            int invocationCount = 0;
            Nasal mocker = new();
            mocker.Method(() => HasPublicMethod.ReturnValue())
                  .Returns(() =>
                  {
                      invocationCount++;
                      return 3;
                  })
                  .CallOriginal();

            using IDisposable _ = await mocker.ApplyAsync();
            HasPublicMethod.ReturnValueCalled = false;

            int value = HasPublicMethod.ReturnValue();

            Assert.Equal(1, invocationCount);
            Assert.Equal(42, value);
            Assert.True(HasPublicMethod.ReturnValueCalled);
        }

    }
}
