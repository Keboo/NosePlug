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
        public async Task CanReplaceLinqWhereMethod()
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

        
    }
}
