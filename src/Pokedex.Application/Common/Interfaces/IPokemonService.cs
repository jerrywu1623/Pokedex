using System.Threading;
using System.Threading.Tasks;
using Pokedex.Domain.Entities;

namespace Pokedex.Application.Common.Interfaces
{
    public interface IPokemonService
    {
        Task<PokemonSpec> GetPokemonSpec(string name, CancellationToken cancellationToken);
    }
}