using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public static class QueryServiceCollectionExtensions
{
    public static IQueryBuilder AddOozeQuery(this IServiceCollection services)
        => new QueryServicesBuilder(services).AddCommonServices();
}