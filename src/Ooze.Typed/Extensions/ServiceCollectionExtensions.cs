using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Extensions;

public static class ServiceCollectionExtensions
{
    public static IOozeServiceCollectionBuilder AddOozeTyped(this IServiceCollection services)
        => new OozeServiceCollectionBuilder(services).AddCommonServices();
}
