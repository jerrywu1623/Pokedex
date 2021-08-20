using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Pokedex.Domain.Entities;
using Pokedex.Infrastructure.Services;

namespace Pokedex.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class ShakespeareTranslationServiceTests
    {
        private Mock<ILogger<ShakespeareTranslationService>> _logger;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private const string BASE_ADDRESS = "https://api.funtranslations.com/translate/";
        private const string TEST_TEXT = "test";

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<ShakespeareTranslationService>>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Test]
        public async Task CallShakespeareApiSuccess()
        {
            //arrange
            var funTranslation = new FunTranslation
            {
                success = new Success { total = 1 },
                contents = new Content
                {
                    translated = "test",
                    text = "test",
                    translation = "shakespeare"
                }
            };
            var fakeHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            fakeHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(funTranslation))
                })
                .Verifiable();
            var client = new HttpClient(fakeHttpMessageHandler.Object);
            client.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            var shakespeareTranslationService = new ShakespeareTranslationService(_httpClientFactory.Object, _logger.Object);
            
            //act
            var result = await shakespeareTranslationService.TranslateAsync(TEST_TEXT);
            
            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task CallShakespeareApiError()
        {
            //arrange
            var fakeHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            fakeHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                })
                .Verifiable();
            var client = new HttpClient(fakeHttpMessageHandler.Object);
            client.BaseAddress = new Uri(BASE_ADDRESS);
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            var shakespeareTranslationService = new ShakespeareTranslationService(_httpClientFactory.Object, _logger.Object);
            
            //act
            var result = await shakespeareTranslationService.TranslateAsync(TEST_TEXT);
            
            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(TEST_TEXT, result);
        }
    }
}