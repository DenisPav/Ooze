using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public interface IQueryBuilder
{
    IQueryBuilder AddQueryProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped);
}