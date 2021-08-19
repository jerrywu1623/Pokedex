using System.Threading;
using System.Threading.Tasks;
using Pokedex.Domain.Enums;

namespace Pokedex.Application.Common.Interfaces
{
    public interface ITranslationService
    {
        TranslationOptions TransactionOption { get; }
        Task<string> TranslateAsync(string text, CancellationToken cancellationToken);
    }
}