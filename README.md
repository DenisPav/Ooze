# Ooze.Typed 🌳💧🔧 
![Nuget](https://img.shields.io/nuget/v/Ooze.Typed)
![framework](https://img.shields.io/badge/framework-.net%207.0-green)
![GitHub Repo stars](https://img.shields.io/github/stars/DenisPav/Ooze)
![Nuget](https://img.shields.io/nuget/dt/Ooze.Typed)

This package provides simple mechanism for applying filters, sorters, paging to your `IQueryable<T>` queries. Packages are available on the `Github package repository` so you can install them from there.

## Installation ⚙
You can find latest versions on nuget [on this location](https://www.nuget.org/packages/Ooze.Typed/).

## Additional filter extensions 🎁
Except base `Ooze.Typed` package there are few more that add additional filter extensions to the filter builder that you use in your provider implementations. These are listed below:
 - [Ooze.Typed.EntityFrameworkCore](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore/)
 - [Ooze.Typed.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Sqlite/)
 - [Ooze.Typed.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.SqlServer/)
 - `Ooze.Typed.EntityFrameworkCore.Psql` - TBD
 - `Ooze.Typed.EntityFrameworkCore.MySql` - TBD

These packages provide additional provider specific `EF` extensions to the filter builder pipeline.

## Registering Ooze 🧰
After installation you'll need to register Ooze to your services collection. To do so you need to call `.AddOozeTyped()` method. Example of this can be seen below:
```csharp
//Example for minimal apis
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOozeTyped();

//Example for Startup class
public class Startup 
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOozeTyped();
    }
    ...
}
```

This call will register internal services needed by `Ooze` and will in turn return a related "builder" via which you can then register your `provider` implementations.

## Adding Filters 🗡️🧀
After registering Ooze you need to create your filter definition. This can be done by implementing `IOozeFilterProvider<TEntity, TFilter>` interface. After creating implementation you can use static `Filters` class to start of the builder which will in turn create your filter definitions. Example can be seen below:
```csharp
public class MyClassFiltersProvider : IOozeFilterProvider<MyClass, MyClassFilters>
{
    public IEnumerable<IFilterDefinition<MyClass, MyClassFilters>> GetFilters()
    {
        return Filters.CreateFor<MyClass, MyClassFilters>()
            //add equality filter onto MyClass instance over Id property and use Id property from incoming filter instance in that operation
            .Equal(x => x.Id, filter => filter.Id)
            //...add other filters if needed
            .Build();
    }
}
```
There are some default filter operations that come when you install `Ooze`. They can be found on example below:
```csharp
public IEnumerable<IFilterDefinition<MyClass, MyClassFilters>> GetFilters()
{
        return Filters.CreateFor<MyClass, MyClassFilters>()
            //check if property is equal to filter
            .Equal(x => x.Id, filter => filter.Id)
            //check if property is different than filter
            .NotEqual(x => x.Id, filter => filter.Id)
            //check if property is greater than filter
            .GreaterThan(x => x.Id, filter => filter.Id)
            //check if property is less than filter
            .LessThan(x => x.Id, filter => filter.Id)
            //check if property is contained in filter array
            .In(x => x.Id, filter => filter.Ids)
            //check if property is not contained in filter array
            .NotIn(x => x.Id, filter => filter.Ids)
            //check if property falls in range of filter
            .Range(x => x.Id, filter => filter.RangeFilter)
            //check if property falls outside of range of filter
            .OutOfRange(x => x.Id, filter => filter.RangeFilter)
            //check if string property starts with filter
            .StartsWith(x => x.Name, filter => filter.Name)
            //check if string property doesn't starts with filter
            .DoesntStartWith(x => x.Id, filter => filter.RangeFilter)
            //check if string property ends with filter
            .EndsWith(x => x.Id, filter => filter.RangeFilter)
            //check if string property doesn't end with filter
            .DoesntEndWith(x => x.Id, filter => filter.RangeFilter)
            //custom filter check, first argument is predicate that tells
            //Ooze if filter should be applied, second is filter factory
            //which takes filter as input and returns wanted filtering
            //expression
            .Add(filter => filter.Name != null, filter => x => x.Name == filter.Name)
            .Build();
}
```
After defining your filter definitions you just need to register class to Ooze via `.Add` method provided
after calling `.AddOozeTyped`. Example can be seen below:
```csharp
services.AddOozeTyped()
    .Add<MyClassFiltersProvider>();
```

By default all the provider implementations that you register via `.Add<TProvider>()` method will be registered to `IServiceCollection` as a `Singleton` but you can provide your own `LifeTime` by providing `ServiceLifetime` argument into the method.

## Adding sorters 🔼🔽
Similarly how you can define filter definitions, you can create sorter definitions which can be then used
by `Ooze` to sort your queries. This is done by implementing `IOozeSorterProvider<TEntity, TSorters>` interface, and using `Sorters` static class to start of builder for creating sorters. Example of this can be found below:
```csharp
public class MyClassSortersProvider : IOozeSorterProvider<MyClass, MyClassSorters>
{
    public IEnumerable<ISortDefinition<MyClass, MyClassSorters>> GetSorters()
    {
        return Sorters.CreateFor<MyClass, MyClassSorters>()
            //add sorting on Id property in provided direction from sorter instance
            .Add(x => x.Id, sort => sort.Id)
            .Build();
    }
}
```
Sorters are added to `Ooze` in same manner as Filters so you can reuse the example mentioned there.

**NOTE:**
Sorters by default use `SortDirection` enumeration in order to specify property sorting direction.

## Paging 📰
Paging is done via `.Page` method on resolver. You just need to pass instance of `PagingOptions` to the before mentioned method. For example:
```csharp
query = resolver
        .WithQuery(query)
        .Page(new PagingOptions
        {
            Page = 0,
            Size = 3,
        })
        .Apply();
```

## Applying definitions 🧪
In order to apply filter/sorter definitions you need to get instance of `IOozeTypedResolver`/`IOozeTypedResolver<TEntity, TFilters, TSorters>` after that you can just call methods in order to change `IQueryable<TEntity>` instance. Here is an more elaborate example below:
```csharp
//lets say you have an route which gets filters/sorters from request body
app.MapPost("/", (
    DatabaseContext db,
    IOozeTypedResolver<MyEntity, MyEntityFilters, MyEntitySorters> resolver,
    Input model) =>
{
    IQueryable<MyEntity> query = db.Set<MyEntity>();

    query = resolver
        .WithQuery(query)
        .Filter(model.Filters)
        //you can also use .Sort(model.Sorters) or .Page(model.Paging) method, or if you don't want to sort or page or even filter something out, you can always remove the calls.
        .Apply();

    return query;
});

public record class Input(MyEntityFilters Filters, MyEntitySorters Sorters, PagingOptions Paging);
```

**NOTE:**
Example before is bound to POST method, but you can use GET or anything else that suits you. For more elaborate example look [here](https://github.com/DenisPav/Ooze/tree/master/tests/Ooze.Typed.Web). Ooze only cares that you provide instances of your `filters`,  `sorters` which will be then applied to `IQueryable` instances.