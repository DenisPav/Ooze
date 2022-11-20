using Microsoft.Extensions.DependencyInjection;

namespace Ooze.Typed.Extensions
{
    public interface IOozeServiceCollectionBuilder
    {
        IServiceCollection Services { get; }
        IOozeServiceCollectionBuilder Add<TProvider>(ServiceLifetime providerLifetime = ServiceLifetime.Singleton);
        IOozeServiceCollectionBuilder AddQueryHandler(Type queryHandlerType);
    }
}
