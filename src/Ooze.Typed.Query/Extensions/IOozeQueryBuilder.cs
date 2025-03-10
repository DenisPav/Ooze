using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public interface IOozeQueryBuilder
{
    IOozeQueryBuilder AddFilterProvider<TProvider>(ServiceLifetime lifetime = ServiceLifetime.Scoped);
}