using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Query.Extensions;

/// <summary>
/// Interface defining contract for adding Query Filter Provider implementations for Ooze QL
/// </summary>
public interface IOozeQueryLanguageBuilder
{
    /// <summary>
    /// Add provider type to Ooze QL pipeline and registers it to <see cref="IServiceCollection"/> with specified lifetime
    /// </summary>
    /// <param name="providerLifetime">Lifetime of provider implementation</param>
    /// <typeparam name="TProvider">Type of provider implementation</typeparam>
    /// <returns>Builder instance</returns>
    IOozeQueryLanguageBuilder AddQueryProvider<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton);
}