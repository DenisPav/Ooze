using Microsoft.Extensions.DependencyInjection;
using Ooze.Typed.Extensions;
using Ooze.Typed.Filters;
using Ooze.Typed.Paging;
using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests;

public class ServiceCollectionTests
{
    [Fact]
    public void Should_Have_Registered_Default_Ooze_Interfaces()
    {
        var services = new ServiceCollection();
        var oozeBuilder = services.AddOozeTyped();
        var requiredInterfaces = new (Type ContractType, Type ImplementationType)[]
        {
            (typeof(IOozeTypedResolver), typeof(OozeTypedResolver)),
            (typeof(IOozeTypedResolver<,>), typeof(OozeTypedResolver<,>)),
            (typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>)),
            (typeof(IOozeSorterHandler<>), typeof(OozeSorterHandler<>)),
            (typeof(IOozePagingHandler<>), typeof(OozePagingHandler<>))
        };

        foreach (var (ContractType, ImplementationType) in requiredInterfaces)
        {
            var count = services.Count(descriptor => descriptor.ServiceType == ContractType
                    && descriptor.ImplementationType == ImplementationType
                    && descriptor.Lifetime == ServiceLifetime.Scoped);

            Assert.True(count == 1);
        }
    }

    [Fact]
    public void Should_Have_Custom_Provider_Implementation()
    {
        var services = new ServiceCollection();
        services.AddOozeTyped()
            .Add<BlogFiltersProvider>()
            .Add<BlogSortersProvider>();

        var requiredInterfaces = new (Type ContractType, Type ImplementationType)[]
        {
            (typeof(IOozeFilterProvider<Blog, BlogFilters>), typeof(BlogFiltersProvider)),
            (typeof(IOozeSorterProvider<Blog>), typeof(BlogSortersProvider)),
        };

        foreach (var (ContractType, ImplementationType) in requiredInterfaces)
        {
            var count = services.Count(descriptor => descriptor.ServiceType == ContractType
                    && descriptor.ImplementationType == ImplementationType
                    && descriptor.Lifetime == ServiceLifetime.Singleton);

            Assert.True(count == 1);
        }
    }
}

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
        => Enumerable.Empty<IFilterDefinition<Blog, BlogFilters>>();
}

public class BlogSortersProvider : IOozeSorterProvider<Blog>
{
    public IEnumerable<ISortDefinition<Blog>> GetSorters()
        => Enumerable.Empty<ISortDefinition<Blog>>();
}