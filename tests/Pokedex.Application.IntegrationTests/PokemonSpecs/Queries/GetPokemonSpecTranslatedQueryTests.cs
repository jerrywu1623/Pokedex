using System.Threading.Tasks;
using NUnit.Framework;
using Pokedex.Application.Common.Exceptions;
using Pokedex.Application.PokemonSpecs.Queries;

namespace Pokedex.Application.IntegrationTests.PokemonSpecs.Queries
{
    [TestFixture]
    public class GetPokemonSpecTranslatedQueryTests: TestBase
    {
        [Test]
        public async Task ShouldReturnYodaSuccess()
        {
            //arrange
            var name = "mewtwo";
            var query = new GetPokemonSpecTranslatedQuery() {name = name};
            
            //act
            var result = await SendAsync(query);
            
            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("yoda_test", result.description);
        }
        
        [Test]
        public async Task ShouldReturnShakespeareSuccess()
        {
            //arrange
            var name = "wormadam";
            var query = new GetPokemonSpecTranslatedQuery() {name = name};
            
            //act
            var result = await SendAsync(query);
            
            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual("shakespeare_test", result.description);
        }

        [Test]
        public async Task ShouldThrowNotFoundException()
        {
            //arrange
            var name = "aaa";
            var query = new GetPokemonSpecTranslatedQuery() {name = name};
            
            //act & assert
            Assert.ThrowsAsync<NotFoundException>(() => SendAsync(query));
        }
    }
}