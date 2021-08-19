using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.Infrastructure.UnitTests
{
    public class FakeTranslationHttpMessageHandler: HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}