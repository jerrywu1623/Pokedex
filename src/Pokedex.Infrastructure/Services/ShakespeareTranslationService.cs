using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Application;
using Pokedex.Application.Common.Enums;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Domain.Entities;

namespace Pokedex.Infrastructure.Services
{
    public class ShakespeareTranslationService: ITranslationService
    {
        private IHttpClientFactory _httpClientFactory;
        private ILogger<ShakespeareTranslationService> _logger;
        private const string SHAKESPEARE_TRANSLATE = "shakespeare";
        
        public ShakespeareTranslationService(IHttpClientFactory httpClientFactory, ILogger<ShakespeareTranslationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public TranslationOptions TransactionOption => TranslationOptions.Shakespeare;

        public async Task<string> TranslateAsync(string text, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("start calling ShakespeareTranslationService");
                var client = _httpClientFactory.CreateClient(Constants.TRANSLATION_API_CLIENT_NAME);
                var requestBody = new { text = text };
                var response = await client.PostAsJsonAsync(SHAKESPEARE_TRANSLATE, requestBody, cancellationToken);

                response.EnsureSuccessStatusCode();
                var funTranslation = await response.Content.ReadFromJsonAsync<FunTranslation>(cancellationToken: cancellationToken);
                
                _logger.LogInformation($"end calling YodaTranslationService, response: {JsonSerializer.Serialize(funTranslation)}");
                return funTranslation.contents.translated;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ShakespeareTranslationService error");
                return text;
            }
        }
    }
}