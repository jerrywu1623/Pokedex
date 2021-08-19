using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pokedex.Infrastructure.Services;
using NSubstitute;
using Pokedex.Domain.Entities;

namespace Pokedex.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class PokemonServiceTests
    {
        private ILogger<PokemonService> _logger;
        private IHttpClientFactory _httpClientFactory;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<PokemonService>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
        }

        [Test]
        public async Task CallPokemonApiSuccess()
        {
            //arrange
            var name = "test";
            var pokemonSpec = new PokemonSpec
            {
                name = "test",
                habitat = new Habitat
                {
                    name = "rare"
                },
                is_legendary = true,
                flavor_text_entries = new []{new Flavor_Text_Entry{flavor_text = "this is test"}}
            };
            var client = new HttpClient(new FakePokemonHttpMessageHandler(pokemonSpec));
            client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
            _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(client);
            var pokemonService = new PokemonService(_logger, _httpClientFactory);

            //act
            var result = await pokemonService.GetPokemonSpec(name);
            
            //assert
            Assert.AreEqual(pokemonSpec.name, result.name);
            Assert.AreEqual(pokemonSpec.habitat.name, result.habitat.name);
            Assert.IsTrue(result.is_legendary);
        }
    }
}