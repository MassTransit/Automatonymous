using System.Threading.Tasks;
using Xunit;

namespace Automatonymous.AsyncTests
{
    public class MyTests
    {
        [Fact]
        public async Task AssertIsTrue()
        {
            Assert.Equal(0.0, await Do());
        }

        static Task<double> Do()
        {
            return Task.FromResult(0.0);
        }
    }
}
