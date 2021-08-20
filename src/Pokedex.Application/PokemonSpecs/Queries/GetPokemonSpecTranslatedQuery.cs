using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Pokedex.Application.Common.Enums;
using Pokedex.Application.Common.Exceptions;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Application.ViewModels;

namespace Pokedex.Application.PokemonSpecs.Queries
{
    public class GetPokemonSpecTranslatedQuery: IRequest<PokemonSpecVm>
    {
        public string name { get; set; }
    }

    public class GetPokemonSpecTranslatedQueryHandler : IRequestHandler<GetPokemonSpecTranslatedQuery, PokemonSpecVm>
    {
        private readonly IPokemonService _pokemonService;
        private readonly IEnumerable<ITranslationService> _translationServices;
        private const string HABITAT_CAVE = "cave";

        public GetPokemonSpecTranslatedQueryHandler(IPokemonService pokemonService, IEnumerable<ITranslationService> translationServices)
        {
            _pokemonService = pokemonService;
            _translationServices = translationServices;
        }

        public async Task<PokemonSpecVm> Handle(GetPokemonSpecTranslatedQuery request, CancellationToken cancellationToken)
        {
            var pokemonSpec = await _pokemonService.GetPokemonSpecAsync(request.name, cancellationToken);
            
            if (pokemonSpec == null)
            {
                throw new NotFoundException($"pokemon not found, name: {request.name}");
            }
            
            var pokemonVm = pokemonSpec.Adapt<PokemonSpecVm>();
            var description = pokemonVm.description;

            if (string.Equals(pokemonVm.habitat, HABITAT_CAVE, StringComparison.OrdinalIgnoreCase) || pokemonVm.isLegendary)
            {
                description = await _translationServices.FirstOrDefault(w => w.TransactionOption == TranslationOptions.Yoda).TranslateAsync(description, cancellationToken);
            }
            else
            {
                description = await _translationServices.FirstOrDefault(w => w.TransactionOption == TranslationOptions.Shakespeare).TranslateAsync(description, cancellationToken);
            }

            pokemonVm.description = Regex.Replace(description, RegexConstants.NEW_LINE, " ");
            return pokemonVm;
        }
    }
}