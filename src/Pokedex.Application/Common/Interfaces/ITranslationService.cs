using System.Threading;
using System.Threading.Tasks;
using Pokedex.Application.Common.Enums;

namespace Pokedex.Application.Common.Interfaces
{
    public interface ITranslationService
    {
        TranslationOptions TranslationOptions { get; }
        Task<string> TranslateAsync(string text, CancellationToken cancellationToken);
    }
}