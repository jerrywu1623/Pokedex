using System.Threading.Tasks;
using NUnit.Framework;
using Pokedex.Application.Common.Exceptions;
using Pokedex.Application.PokemonSpecs.Queries;

namespace Pokedex.Application.IntegrationTests.PokemonSpecs.Queries
{
    [TestFixture]
    public class GetPokemonSpecQueryTests: TestBase
    {
        [Test]
        public async Task ShouldReturnPokemonVmSuccess()
        {
            //arrange
            var query = new GetPokemonSpecQuery { name = "mewtwo" };
            
            //act
            var result = await SendAsync(query);
            
            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task ShouldThrowNotFoundException()
        {
            //arrange
            var query = new GetPokemonSpecQuery { name = "aaa" };
            
            //act & assert
            Assert.ThrowsAsync<NotFoundException>(() => SendAsync(query));
        }
    }
}