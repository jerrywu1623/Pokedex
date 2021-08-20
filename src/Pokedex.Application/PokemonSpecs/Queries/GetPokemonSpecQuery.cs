using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Pokedex.Application.Common.Exceptions;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Application.ViewModels;

namespace Pokedex.Application.PokemonSpecs.Queries
{
    public class GetPokemonSpecQuery: IRequest<PokemonSpecVm>
    {
        public string name { get; set; }
    }

    public class GetPokemonSpecQueryHandler : IRequestHandler<GetPokemonSpecQuery, PokemonSpecVm>
    {
        private readonly IPokemonService _pokemonService;

        public GetPokemonSpecQueryHandler(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        public async Task<PokemonSpecVm> Handle(GetPokemonSpecQuery request, CancellationToken cancellationToken)
        {
            var pokemonSpec = await _pokemonService.GetPokemonSpecAsync(request.name, cancellationToken);

            if (pokemonSpec == null)
            {
                throw new NotFoundException($"pokemon not found, name: {request.name}");
            }

            var pokemonSpecVm = pokemonSpec.Adapt<PokemonSpecVm>();
            pokemonSpecVm.description = Regex.Replace(pokemonSpecVm.description, RegexConstants.NEW_LINE, " ");
            return pokemonSpecVm;
        }
    }
}