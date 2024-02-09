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
        services.AddOozeTyped();
        var requiredInterfaces = new (Type ContractType, Type ImplementationType)[]
        {
            (typeof(IOozeTypedResolver), typeof(OozeTypedResolver)),
            (typeof(IOozeTypedResolver<,,>), typeof(OozeTypedResolver<,,>)),
            (typeof(IOozeFilterHandler<,>), typeof(OozeFilterHandler<,>)),
            (typeof(IOozeSorterHandler<,>), typeof(OozeSorterHandler<,>)),
            (typeof(IOozePagingHandler<>), typeof(OozePagingHandler<>))
        };

        foreach (var (contractType, implementationType) in requiredInterfaces)
        {
            var count = services.Count(descriptor => descriptor.ServiceType == contractType
                    && descriptor.ImplementationType == implementationType
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
            (typeof(IOozeSorterProvider<Blog, BlogSorters>), typeof(BlogSortersProvider)),
        };

        foreach (var (contractType, implementationType) in requiredInterfaces)
        {
            var count = services.Count(descriptor => descriptor.ServiceType == contractType
                    && descriptor.ImplementationType == implementationType
                    && descriptor.Lifetime == ServiceLifetime.Singleton);

            Assert.True(count == 1);
        }
    }

    [Fact]
    public void Should_Have_Custom_Provider_Implementations_If_Both_Providers_Are_Implemented()
    {
        var services = new ServiceCollection();
        services.AddOozeTyped()
            .Add<BlogFiltersAndSortersProvider>();

        var requiredInterfaces = new (Type ContractType, Type ImplementationType)[]
        {
            (typeof(IOozeFilterProvider<Blog, BlogFilters>), typeof(BlogFiltersAndSortersProvider)),
            (typeof(IOozeSorterProvider<Blog, BlogSorters>), typeof(BlogFiltersAndSortersProvider)),
        };

        foreach (var (contractType, implementationType) in requiredInterfaces)
        {
            var count = services.Count(descriptor => descriptor.ServiceType == contractType
                                                     && descriptor.ImplementationType == implementationType
                                                     && descriptor.Lifetime == ServiceLifetime.Singleton);

            Assert.True(count == 1);
        }
    }

    [Fact]
    public void Should_Fail_If_Non_Provider_Type_Is_Used()
    {
        var services = new ServiceCollection();
        var oozeBuilder = services.AddOozeTyped();

        Assert.Throws<ArgumentException>(() => oozeBuilder.Add<int>());
    }
}

public class BlogFiltersProvider : IOozeFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
        => Enumerable.Empty<IFilterDefinition<Blog, BlogFilters>>();
}

public class BlogSortersProvider : IOozeSorterProvider<Blog, BlogSorters>
{
    public IEnumerable<ISortDefinition<Blog, BlogSorters>> GetSorters()
        => Enumerable.Empty<ISortDefinition<Blog, BlogSorters>>();
}

public class BlogFiltersAndSortersProvider : IOozeFilterProvider<Blog, BlogFilters>, IOozeSorterProvider<Blog, BlogSorters>
{
    public IEnumerable<IFilterDefinition<Blog, BlogFilters>> GetFilters()
        => Enumerable.Empty<IFilterDefinition<Blog, BlogFilters>>();
    public IEnumerable<ISortDefinition<Blog, BlogSorters>> GetSorters()
        => Enumerable.Empty<ISortDefinition<Blog, BlogSorters>>();
}