# Ooze.Typed 🌳💧🔧
[![Nuget](https://img.shields.io/nuget/v/Ooze.Typed)](https://www.nuget.org/packages/Ooze.Typed/)
![framework](https://img.shields.io/badge/framework-.net%208.0-green)
![framework](https://img.shields.io/badge/framework-.net%209.0-green)
![GitHub Repo stars](https://img.shields.io/github/stars/DenisPav/Ooze)
![Nuget](https://img.shields.io/nuget/dt/Ooze.Typed)
![Coverage](CoverageReport/badge_combined.svg)

**Ooze.Typed** is a .NET library that simplifies data querying in your applications by providing a strongly-typed approach to filtering, sorting, and paging operations on `IQueryable<T>` sources. Key features of library are:
- **Strongly-typed filters/sorters**
- **Pagination**
- **Query language** - Optional support for string-based query expressions
- **Async capabilities** - Optional opt in async/await support

## Table of Contents
  - [Installation](#installation)
    - [Additional packages](#additional-packages)
  - [Documentation](#documentation)
    - [Creating filters](#creating-filters)
    - [Creating sorters](#creating-sorters)
    - [Registering implementations](#registering-implementations)
    - [Applying filtering/sorting/paging](#applying-filteringsortingpaging)
    - [More information](#more-information)
  - [Filter extensions](#filter-extensions)
    - [Ooze.Typed.EntityFrameworkCore](#oozetypedentityframeworkcore)
    - [Ooze.Typed.EntityFrameworkCore.Sqlite](#oozetypedentityframeworkcoresqlite)
    - [Ooze.Typed.EntityFrameworkCore.SqlServer](#oozetypedentityframeworkcoresqlserver)
    - [Ooze.Typed.EntityFrameworkCore.Npgsql](#oozetypedentityframeworkcorenpgsql)
    - [Ooze.Typed.EntityFrameworkCore.MySql](#oozetypedentityframeworkcoremysql)

## Installation
You can find latest versions on nuget [on this location](https://www.nuget.org/packages/Ooze.Typed/).

### Additional packages
Except base `Ooze.Typed` package there are few more that add additional filter extensions to the filter builder that you use in your provider implementations. These are listed below:
 - [Ooze.Typed.EntityFrameworkCore](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore/)
 - [Ooze.Typed.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Sqlite/)
 - [Ooze.Typed.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.SqlServer/)
 - [Ooze.Typed.EntityFrameworkCore.Npgsql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Npgsql/)
 - [Ooze.Typed.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.MySql/)

These packages provide additional provider specific `EF` extensions to the filter builder pipeline. There is another package which can be installed and it will provide `query language` filtration:
 - [Ooze.Typed.Query](https://www.nuget.org/packages/Ooze.Typed.Query/)

## Documentation
In order to use Ooze you'll need to create a few things. Simple example will be shown in the subsections below.

### Creating filters
First off, in order to be able to filter your `IQueryable<T>` queries, you will need an implementation of `IFilterProvider` interface, a filter type and queryable entity type. Example of this can be seen below:
```csharp
public class MyClass 
{
    public int Id { get; set; }
}

public class MyClassFilters
{
    public int Id { get; set; }
}


public class MyClassFiltersProvider : IFilterProvider<MyClass, MyClassFilters>
{
    public IEnumerable<FilterDefinition<MyClass, MyClassFilters>> GetFilters()
    {
        return Filters.CreateFor<MyClass, MyClassFilters>()
            //add equality filter onto MyClass instance over Id property and use Id property from incoming filter instance in that operation
            .Equal(x => x.Id, filter => filter.Id)
            //...add other filters if needed
            .Build();
    }
}
```

### Creating sorters
Additionally, if we need to support sorting of `IQueryable<T>` queries, we will need implementation of `ISorterProvider` which will look similar to the filter implementation. Example of sorter provider can be seen below:
```csharp
public class MyClassSortersProvider : ISorterProvider<MyClass, MyClassSorters>
{
    public IEnumerable<SortDefinition<MyClass, MyClassSorters>> GetSorters()
    {
        return Sorters.CreateFor<MyClass, MyClassSorters>()
            //add sorting on Id property in provided direction from sorter instance
            .SortBy(x => x.Id, sort => sort.Id)
            .Build();
    }
}
```

### Registering implementations
Next step would be to register your filter/sorter implementations in your service collection. Example of this can be seen below:
```csharp
//Example for minimal apis
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOozeTyped()
    .Add<MyClassFiltersProvider>()
    .Add<MyClassSortersProvider>();

//Example for Startup class
public class Startup 
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOozeTyped()
            .Add<MyClassFiltersProvider>()
            .Add<MyClassSortersProvider>();
    }
    ...
}
```

### Applying filtering/sorting/paging
Last part of this would be to call `IOperationResolver` in your code and pass it the filtering/sorting/paging options that you want to apply to your `IQueryable<T>` instance. Example of this can be seen below:

```csharp
//lets say you have a route which gets filters/sorters from request body
app.MapPost("/", (
    DatabaseContext db,
    IOperationResolver<MyEntity, MyEntityFilters, MyEntitySorters> resolver,
    Input model) =>
{
    IQueryable<MyEntity> query = db.Set<MyEntity>();

    query = resolver
        .WithQuery(query)
        .Filter(model.Filters)
        //you can also use .Sort(model.Sorters) or .Page(model.Paging) method, or if you don't want to sort or page or even filter something out, you can always remove the calls.
        .Apply();

    //query will now contain updated IQueryable<MyEntity> instance with applied filters that were passed in the request
    return query;
});

//example type holding different details needed for filters/sorters/paging
public record Input(MyEntityFilters Filters, MyEntitySorters Sorters, PagingOptions Paging);
```

And that is it. You can define from which place you want to map/bind filters, sorters, paging. If you have a POST/PUT endpoint you can map it from request body. In case of GET request you might get it from the request url. Or in case you got some service you might manually pass the filters down to `IOperationResolver`. You can use whatever suits you best here.

### More information
In case you want to see more examples and a bit more detailed documentation about each of the components be sure to check out [docs](docs/info.md)

## Filter extensions
As previously mentioned additional packages contains some usefull extensions when working with specific "flavor" of EF. For example you might be using `Sqlite` or `SqlServer` or `Postgres` etc. For these situations you can install these specific packages which contain extensions methods for the specific flavor. More about what is supported on each of the packages can be seen below.

### [Ooze.Typed.EntityFrameworkCore](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore/)
This packages depends on EF Core packages and exposes next extensions:
 - `Like()` - EF.Eunctions.Like

### [Ooze.Typed.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Sqlite/)
This packages depends on EF Core Sqlite package and package mentioned beforehand and exposes next extensions:
 - `Glob()` - EF.Functions.Glob

### [Ooze.Typed.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.SqlServer/)
This package depends on EF Core SqlServer package and package mentioned beforehand and exposes next extensions:
 - `IsDate()` - EF.Functions.IsDate
 - `IsNumeric()` - EF.Functions.IsNumeric
 - `Contains()` - EF.Functions.Contains
 - `IsDateDiffDay()` - EF.Functions.DateDiffDay
 - `IsDateDiffMonth()` - EF.Functions.DateDiffMonth
 - `IsDateDiffWeek()` - EF.Functions.DateDiffWeek
 - `IsDateDiffYear()` - EF.Functions.DateDiffYear
 - `IsDateDiffHour()` - EF.Functions.DateDiffHour
 - `IsDateDiffMinute()` - EF.Functions.DateDiffMinute
 - `IsDateDiffSecond()` - EF.Functions.DateDiffSecond
 - `IsDateDiffMilisecond()` - EF.Functions.DateDiffMilisecond
 - `IsDateDiffMicrosecond()` - EF.Functions.DateDiffMicrosecond
 - `IsDateDiffNanosecond()` - EF.Functions.DateDiffNanosecond

### [Ooze.Typed.EntityFrameworkCore.Npgsql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Npgsql)
This package depends on EF Core Npgsql package and package mentioned beforehand and exposes next extensions:
 - `InsensitiveLike()` - EF.Functions.ILike
 - `SoundexEqual()` - EF.Functions.FuzzyStringMatchSoundex

### [Ooze.Typed.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.MySql)
This package depends on EF Core MySql (Pomelo) package and package mentioned beforehand and exposes next extensions:
- `IsDateDiffDay()` - EF.Functions.DateDiffDay
- `IsDateDiffMonth()` - EF.Functions.DateDiffMonth
- `IsDateDiffYear()` - EF.Functions.DateDiffYear
- `IsDateDiffHour()` - EF.Functions.DateDiffHour
- `IsDateDiffMinute()` - EF.Functions.DateDiffMinute
- `IsDateDiffSecond()` - EF.Functions.DateDiffSecond
- `IsDateDiffMicrosecond()` - EF.Functions.DateDiffMicrosecond
 