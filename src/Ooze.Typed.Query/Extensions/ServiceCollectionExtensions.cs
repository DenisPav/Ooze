using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

/// <summary>
/// Static class with extensions for <see cref="IServiceCollection"/> for easier Ooze query language integration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Ooze query language and related support services to the <see cref="IServiceCollection"/> instance
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
    /// <returns>Ooze query language builder</returns>
    public static IOozeQueryLanguageBuilder AddOozeQueryLanguage(this IServiceCollection services)
        => new OozeQueryLanguageServicesBuilder(services).AddCommonServices();
}