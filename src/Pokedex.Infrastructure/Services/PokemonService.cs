using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pokedex.Application;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Domain.Entities;

namespace Pokedex.Infrastructure.Services
{
    public class PokemonService: IPokemonService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PokemonService> _logger;
        private const string POKEMON_SPEC_URI = "/api/v2/pokemon-species/{0}";

        public PokemonService(ILogger<PokemonService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PokemonSpec> GetPokemonSpec(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Constants.POKEMON_API_CLIENT_NAME);
                var pokemonSpec = await client.GetFromJsonAsync<PokemonSpec>(string.Format(POKEMON_SPEC_URI, name), cancellationToken);
                return pokemonSpec;
            }
            catch(HttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "calling PokemonService error, name: {0}", name);
                throw;
            }
        }
    }
}