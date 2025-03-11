using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public interface IQueryBuilder
{
    IQueryBuilder AddFilterProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped);
}