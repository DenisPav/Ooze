using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Extensions;

public interface IOozeServiceCollectionBuilder
{
    IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton);
}
