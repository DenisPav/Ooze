using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

public static class QueryLanguageServiceCollectionExtensions
{
    public static IQueryLanguageBuilder AddOozeQueryLanguage(this IServiceCollection services)
        => new QueryLanguageServicesBuilder(services).AddCommonServices();
}