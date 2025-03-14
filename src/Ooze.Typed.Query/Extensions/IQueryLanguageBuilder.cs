using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public interface IQueryLanguageBuilder
{
    IQueryLanguageBuilder AddQueryProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped);
}