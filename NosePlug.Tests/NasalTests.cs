using System;
using System.Threading.Tasks;
using Xunit;

namespace NosePlug.Tests
{
    public class NasalTests
    {
        [Fact]
        public async Task IsItMyBirthday()
        {
            DateTime today = DateTime.Today;

            using (await Smell<DateTime>.PlugProperty(
                nameof(DateTime.Today), () => new DateTime(1987, 4, 20)))
            {
                Assert.Equal(new DateTime(1987, 4, 20), DateTime.Today);
            }

            Assert.NotEqual(new DateTime(1987, 4, 20), DateTime.Today);
            Assert.Equal(today, DateTime.Today);
        }

        [Fact]
        public async Task AhToBeYoungAgain()
        {
            using (await Smell<Task>.PlugMethod(() => Task.Run((Func<int>)null!),
                    () => Task.FromResult(42)))
            {
                Assert.Equal(42, await Task.Run(() => 21));
            }

            Assert.Equal(21, await Task.Run(() => 21));
        }

        [Fact(Skip = "Just testing syntax")]
        public async Task TestSyntax()
        {
            //Arrange
            Nasal mocker = new();
            mocker.PlugMethod(() => Task.Run((Func<int>)null!), () => Task.FromResult(42));
            mocker.PlugProperty<DateTime>(nameof(DateTime.Today), () => new DateTime(1987, 4, 20));

            using IDisposable _ = await mocker.ApplyAsync();

            //Act


            //Assert
        }
    }
}
