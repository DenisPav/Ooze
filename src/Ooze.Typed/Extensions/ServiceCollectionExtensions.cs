using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Extensions;

/// <summary>
/// Static class with extensions for <see cref="IServiceCollection"/> for easier Ooze integration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Ooze and related support services to the <see cref="IServiceCollection"/> instance
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
    /// <returns>Ooze builder</returns>
    public static IOozeServiceCollectionBuilder AddOozeTyped(this IServiceCollection services)
        => new OozeServiceCollectionBuilder(services).AddCommonServices();
}
