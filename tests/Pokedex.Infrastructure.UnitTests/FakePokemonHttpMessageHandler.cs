using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pokedex.Domain.Entities;

namespace Pokedex.Infrastructure.UnitTests
{
    public class FakePokemonHttpMessageHandler: HttpMessageHandler
    {
        private PokemonSpec _pokemonSpec;

        public FakePokemonHttpMessageHandler(PokemonSpec pokemonSpec)
        {
            _pokemonSpec = pokemonSpec;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(_pokemonSpec))
            });
        }
    }
}