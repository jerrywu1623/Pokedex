using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Application.PokemonSpecs.Queries;
using Pokedex.Application.ViewModels;

namespace Pokedex.Web.Controllers
{
    public class PokemonsController : ApiControllerBase
    {
        [HttpGet("{name}")]
        public async Task<ActionResult<PokemonSpecVm>> Get(string name, CancellationToken cancellationToken)
        {
            return await Mediator.Send(new GetPokemonSpecQuery() { name = name }, cancellationToken);
        }

        [HttpGet]
        [Route("translated/{name}")]
        public async Task<ActionResult<PokemonSpecVm>> GetTranslated(string name, CancellationToken cancellationToken)
        {
            return await Mediator.Send(new GetPokemonSpecTranslatedQuery() { name = name });
        }
    }
}