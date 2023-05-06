# Ooze.Typed 🌳💧🔧 
This package provides simple mechanism for applying filters, sorters, paging to your `IQueryable<T>` queries. Packages are available on the `Github package repository` so you can install them from there.

## Installation ⚙
First you'll need to install package from `Github package repository`. In order to do so follow directions on the [official github documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry). You'll need to generate Github personal access token and add new nuget source in order to install package from github. More information can be found on the before mentioned page.

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

## Adding Filters 🗡️🧀
After registering Ooze you need to create your filter definition. This can be done by implementing `IOozeFilterProvider<TEntity, TFilter>` interface. After impementing you can use static `Filters` class to start of the builder which will in turn create your filter definitions. Example can be seen below:
```csharp
public class MyClassFiltersProvider : IOozeFilterProvider<MyClass, MyClassFilters>
{
    public IEnumerable<IFilterDefinition<MyClass, MyClassFilters>> GetFilters()
    {
        return Filters.CreateFor<MyClass, MyClassFilters>()
            .Equal(x => x.Id, filter => filter.Id)
            //...add other filters if needed
            .Build();
    }
}
```
There are some default filter operations that come when you install Ooze. They can be found on example below:
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

## Adding sorters 🔼🔽
Similarly how you can define filter definitions, you can create sorter definitions which can be then used
by `Ooze` to sort your queries. This is done by implementing `IOozeSorterProvider<TEntity, TSorter>` interface, and using `Sorters` static class to start of builder for creating sorters. Example of this can be found below:
```csharp
public class MyClassSortersProvider : IOozeSorterProvider<MyClass, MyClassSorters>
{
    public IEnumerable<ISortDefinition<MyClass, MyClassSorters>> GetSorters()
    {
        return Sorters.CreateFor<MyClass, MyClassSorters>()
            //add sorting on Id property in provided direction
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
In order to apply filter/sorter definitions you need to get instance of `IOozeTypedResolver`/`IOozeTypedResolver<TEntity, TFilters>` after that you can just call methods in order to change `IQueryable<TEntity>` instance. Here is an more elaborate example below:
```csharp
//lets say you have an route which gets filters/sorters from request body
app.MapPost("/", (
    DatabaseContext db,
    IOozeTypedResolver<MyEntity, MyEntityFilters> resolver,
    Input model) =>
{
    IQueryable<MyEntity> query = db.Set<MyEntity>();

    query = resolver
        .WithQuery(query)
        .Filter(model.Filters)
        //you can also .Sort(model.Sorters) or .Page(model.Paging) this query if needed
        .Apply();

    return query;
});

public record class Input(BlogFilters Filters, IEnumerable<Sorter> Sorters);
```

**NOTE:**
Example before is bound to POST method, but you can use GET or anything else that suits you. For more elaborate example look [here](https://github.com/DenisPav/Ooze/tree/master/tests/Ooze.Typed.Web).