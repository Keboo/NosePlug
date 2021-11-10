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
        }
    }
}
