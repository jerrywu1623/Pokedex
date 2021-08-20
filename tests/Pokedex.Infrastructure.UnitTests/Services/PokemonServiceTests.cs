using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Pokedex.Infrastructure.Services;
using Pokedex.Domain.Entities;
using Moq;
using Moq.Protected;

namespace Pokedex.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class PokemonServiceTests
    {
        private Mock<ILogger<PokemonService>> _logger;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private const string BASE_ADDRESS = "https://pokeapi.co/api/v2/";
        private const string TEST_POKEMON_NAME = "test";

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<PokemonService>>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Test]
        public async Task CallPokemonApiSuccess()
        {
            //arrange
            var pokemonSpec = new PokemonSpec
            {
                name = TEST_POKEMON_NAME,
                habitat = new Habitat
                {
                    name = "rare"
                },
                is_legendary = true,
                flavor_text_entries = new []{new Flavor_Text_Entry{flavor_text = "this is test"}}
            };
            var fakeHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            fakeHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(pokemonSpec))
                })
                .Verifiable();
            var client = new HttpClient(fakeHttpMessageHandler.Object);
            client.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            var pokemonService = new PokemonService(_logger.Object, _httpClientFactory.Object);

            //act
            var result = await pokemonService.GetPokemonSpecAsync(TEST_POKEMON_NAME);
            
            //assert
            Assert.AreEqual(pokemonSpec.name, result.name);
            Assert.AreEqual(pokemonSpec.habitat.name, result.habitat.name);
            Assert.IsTrue(result.is_legendary);
        }

        [Test]
        public async Task CallPokemonServiceGetNull()
        {
            //arrange
            var fakeHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            fakeHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                })
                .Verifiable();
            var client = new HttpClient(fakeHttpMessageHandler.Object);
            client.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            var pokemonService = new PokemonService(_logger.Object, _httpClientFactory.Object);
            
            
            //act
            var result = await pokemonService.GetPokemonSpecAsync(TEST_POKEMON_NAME);
            
            //assert
            Assert.IsNull(result);
        }

        [Test]
        public void CallPokemonServiceThrowException()
        {
            //arrange
            var fakeHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            fakeHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError
                })
                .Verifiable();
            var client = new HttpClient(fakeHttpMessageHandler.Object);
            client.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            var pokemonService = new PokemonService(_logger.Object, _httpClientFactory.Object);
            
            
            //act & assert
            Assert.ThrowsAsync<HttpRequestException>(() => pokemonService.GetPokemonSpecAsync(TEST_POKEMON_NAME));
        }
    }
}