using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Extensions;

/// <summary>
/// Interface defining contract for adding Provider implementations for Ooze
/// </summary>
public interface IOozeServiceCollectionBuilder
{
    /// <summary>
    /// Add provider type to Ooze pipeline and registers it to <see cref="IServiceCollection"/> with specified lifetime
    /// </summary>
    /// <param name="providerLifetime">Lifetime of provider implementation</param>
    /// <typeparam name="TProvider">Type of provider implementation</typeparam>
    /// <returns>Builder instance</returns>
    IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton);
}
