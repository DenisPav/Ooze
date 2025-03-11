using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public static class QueryServiceCollectionExtensions
{
    public static IQueryBuilder AddQuery(this IServiceCollection services)
        => new QueryServicesBuilder(services);
}