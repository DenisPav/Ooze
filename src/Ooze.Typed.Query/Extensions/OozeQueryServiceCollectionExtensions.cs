using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public static class OozeQueryServiceCollectionExtensions
{
    public static IOozeQueryBuilder AddOozeQuery(this IServiceCollection services)
        => new OozeQueryBuilder(services);
}