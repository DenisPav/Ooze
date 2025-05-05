# Ooze.Typed 🌳💧🔧
[![Nuget](https://img.shields.io/nuget/v/Ooze.Typed)](https://www.nuget.org/packages/Ooze.Typed/)
![framework](https://img.shields.io/badge/framework-.net%208.0-green)
![framework](https://img.shields.io/badge/framework-.net%209.0-green)
![GitHub Repo stars](https://img.shields.io/github/stars/DenisPav/Ooze)
![Nuget](https://img.shields.io/nuget/dt/Ooze.Typed)

## Table of Contents
- [About](#about)
- [Installation](#installation-)
- [Additional Packages](#additional-packages-)
- [Documentation](#documentation-)
  - [Registering Ooze](#registering-ooze-)
  - [Adding Filters](#adding-filters-️)
  - [Adding Sorters](#adding-sorters-)
  - [Paging](#paging-)
      - [Cursor Paging](#cursor-paging-)
  - [Applying Definitions](#applying-definitions-)
  - [Async Support](#async-support-)
  - [Query Language](#query-language-)
  - [Advanced](#advanced-)
      - [Controlling Filter Builders/Providers](#controlling-filter-buildersproviders)
      - [Automatic Application in MVC via ResultFilter](#automatic-application-in-mvc-via-resultfilter)
      - [Applying Filters and Sorters on IEnumerable Collections](#applying-filters-and-sorters-on-ienumerable-collections)
- [Filter Extensions](#filter-extensions-)
    - [Ooze.Typed.EntityFrameworkCore](#oozetypedentityframeworkcore)
    - [Ooze.Typed.EntityFrameworkCore.Sqlite](#oozetypedentityframeworkcoresqlite)
    - [Ooze.Typed.EntityFrameworkCore.SqlServer](#oozetypedentityframeworkcoresqlserver)
    - [Ooze.Typed.EntityFrameworkCore.Npgsql](#oozetypedentityframeworkcorenpgsql)
    - [Ooze.Typed.EntityFrameworkCore.MySql](#oozetypedentityframeworkcoremysql)


## About

**Ooze.Typed** is a .NET library that simplifies data querying in your applications by providing a strongly-typed approach to filtering, sorting, and paging operations on `IQueryable<T>` sources. Key features of library are:
- **Strongly-typed filters/sorters**
- **Pagination**
- **Query language** - Optional support for string-based query expressions
- **Async capabilities** - Optional opt in async/await support

## Installation
You can find latest versions on nuget [on this location](https://www.nuget.org/packages/Ooze.Typed/).

## Additional packages
Except base `Ooze.Typed` package there are few more that add additional filter extensions to the filter builder that you use in your provider implementations. These are listed below:
 - [Ooze.Typed.EntityFrameworkCore](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore/)
 - [Ooze.Typed.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Sqlite/)
 - [Ooze.Typed.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.SqlServer/)
 - [Ooze.Typed.EntityFrameworkCore.Npgsql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Npgsql/)
 - [Ooze.Typed.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.MySql/)

These packages provide additional provider specific `EF` extensions to the filter builder pipeline. There is another package which can be installed and it will provide `query language` filtration:
 - [Ooze.Typed.Query](https://www.nuget.org/packages/Ooze.Typed.Query/)

## Documentation

### Registering Ooze
After installation you'll need to register Ooze to your service collection. To do so you need to call `.AddOozeTyped()` method. Example of this can be seen below:
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

### Adding Filters
After registering Ooze you need to create your filter definition. This can be done by implementing `IFilterProvider<TEntity, TFilter>` interface. After creating implementation you can use static `Filters` class to start of the builder which will in turn create your filter definitions. Example can be seen below:
```csharp
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
There are some default filter operations that come when you install `Ooze`. They can be found on example below:
```csharp
public IEnumerable<FilterDefinition<MyClass, MyClassFilters>> GetFilters()
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
            .DoesntStartWith(x => x.Name, filter => filter.Name)
            //check if string property ends with filter
            .EndsWith(x => x.Name, filter => filter.Name)
            //check if string property doesn't end with filter
            .DoesntEndWith(x => x.Name, filter => filter.Name)
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

### Adding sorters
Similarly how you can define filter definitions, you can create sorter definitions which can be then used
by `Ooze` to sort your queries. This is done by implementing `ISorterProvider<TEntity, TSorters>` interface, and using `Sorters` static class to start of builder for creating sorters. Example of this can be found below:
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
Sorters are added to `Ooze` in same manner as Filters so you can reuse the example mentioned there.

**NOTE:**
Sorters by default use `SortDirection` enumeration in order to specify property sorting direction.

### Paging
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

#### Cursor paging
There is also additional support for cursor paging via `.PageWithCursor` method on the resolver. If you want to use cursor paging you'll be using `CursorPagingOptions<T>` in this case and you'll need to pass a property you want to use as a cursor source. Example of this can be found in the next example:
```csharp
query = resolver
        .WithQuery(query)
        .PageWithCursor(
            entity => entity.Id,
            new CursorPagingOptions<int>
            {
                After = 0,
                Size = 3,
            }
        )
        .Apply();
```

### Applying definitions
In order to apply filter/sorter definitions you need to get instance of `IOperationResolver`/`IOperationResolver<TEntity, TFilters, TSorters>` after that you can just call methods in order to change `IQueryable<TEntity>` instance. Here is a more elaborate example below:
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

    return query;
});

public record class Input(MyEntityFilters Filters, MyEntitySorters Sorters, PagingOptions Paging);
```

**NOTE:**
Example before is bound to POST method, but you can use GET or anything else that suits you. For more elaborate example look [here](https://github.com/DenisPav/Ooze/tree/master/tests/Ooze.Typed.Web). Ooze only cares that you provide instances of your `filters`,  `sorters` which will be then applied to `IQueryable` instances.

### Async support
If needed you can opt in for the `async` version of the pipeline for the resolvers for different operations. In order to opt into async support you'll need to call `EnableAsyncResolvers()` call on the `IOozeServiceCollectionBuilder` which is exposed when calling `.AddOoze()` extension. Then you can just register providers as before via `.Add<TProvider>()` method.

In order for `FilterProvider` or `SorterProvider` to be of `async` nature you need to use `IAsyncFilterProvider` or `IAsyncSorterProvider` interfaces. Accompanying `AsyncFilters` and `AsyncSorters` static classes are present to help you out with the building process as they are in non async version.

In the end you'll need to use `IAsyncOperationResolver` istead of `IOperationResolver` and that should be it. Example of this can be seen below:
```csharp
//provider definition
public class MyEntityAsyncFiltersProvider : IAsyncFilterProvider<MyEntity, MyEntityFilters>
{
    public ValueTask<IEnumerable<AsyncFilterDefinition<MyEntity, MyEntityFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<MyEntity, MyEntityFilters>()
            .Equal(entity => entity.Id, filter => filter.PostId)
            .NotEqual(entity => entity.Id, filter => filter.NotEqualPostId)
            .AddAsync(async filters =>
            {
                await Task.CompletedTask;
                return filters.Date != null;
            }, async filters =>
            {
                await Task.CompletedTask;
                return entity => entity.Date == filters.Date;
            })
            .Build());
}


//in Program.cs or where your service collection registration is:
builder.Services.AddOozeTyped()
    .EnableAsyncResolvers()
    .Add<MyEntityAsyncFiltersProvider>();

//in your endpoints/controller actions/middlewares call the appropriate resolver methods:
[HttpPost("/sql-server")]
public async Task<IActionResult> PostSqlServer(
    [FromServices] IAsyncOperationResolver<MyEntity, MyFilters, MySorters> asyncResolver, 
    Input model)
    {
        IQueryable<MyEntity> query = _sqlServerDb.Set<MyEntity>();

        query = await asyncResolver.WithQuery(query)
            .Filter(model.Filters)
            .Sort(model.Sorters)
            .ApplyAsync();

        var results = await query.ToListAsync();
        return Ok(results);
    }

record class Input(MyFilters Filters, IEnumerable<MySorters> Sorters, PagingOptions Paging);
```

**NOTE:**
`AsyncFilters/Sorters` builders will currently internally wrap the operations into `Tasks` even if they initially do not look like ones.

### Query language
Ooze supports a subset of operations to be used via `Query Language`. In order to use/support these you will need to installed mentioned package from the `Additional Packages` section (`Ooze.Typed.Query`). In order to register related implementations be sure to call next method:

```csharp
// Example for minimal APIs registration
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOozeTyped()
    .AddOozeQueryLanguage()
    .AddQueryProvider<BlogQueryLanguageProvider>();

// Example for Startup class registration
public class Startup 
{
    ...
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOozeTyped()
            .AddOozeQueryLanguage()
            .AddQueryProvider<BlogQueryLanguageProvider>();
    }
    ...
}

```

Calling `.AddOozeQueryLanguage()` will register `IQueryLanguageOperationResolver` to services collection which can then be used to filter `IQueryable<T>` instances with query language.

Package provides `IQueryLanguageFilterProvider<TEntity>` which can be used to create implementations of query language filter providers. Similar to how ordinary filter providers in Ooze work, you will have access to `QueryLanguageFilters` static class which can be used to start the process of creating a query language registration. Example of this can be seen below:

```csharp
public class BlogFiltersProvider : IQueryLanguageFilterProvider<Blog>
{
    public IEnumerable<QueryLanguageFilterDefinition<Blog>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Blog>()
            .Add(x => x.Id)
            .Add(x => x.Rating)
            .Add(x => x.Name, "Status")
            .Build();
    }
}
```

Each query language filter registration for entity can be customized with unique name or it will be inherited from the expression provided to `.Add()` method.

Query language supports next operations:
- **Equal**: `property == 'value'`
- **Not Equal**: `property != 'value'`
- **Greater Than**: `property >> 'value'`
- **Greater Than or Equal**: `property >= 'value'`
- **Less Than**: `property << 'value'`
- **Less Than or Equal**: `property <= 'value'`
- **Contains**: `property %% 'value'`
- **Starts With**: `property @= 'value'`
- **Ends With**: `property =@ 'value'`
- **Logical Operators**: `&&` (AND), `||` (OR)
- **Grouping**: (  )

Few examples of QL queries for provider declared in previous examples can be seen below:
```
Id == '3'

Id >> '3' && Status == 'Blog 1'

Id << '100' && (Status @= 'Blog' || Rating >= '5')
```

Example of using query language can be seen below:
```csharp
var resultingQueryable = resolver
    .WithQuery(query)
    .FilterWithQueryLanguage("price << 1000 && category == 'Electronics'")
    .Sort(sorterOptions)
    .Apply();
```

### Advanced
 
#### Controlling filter builders/providers

Filter builders have a special parameter called `shouldRun` which is by default nullable. You can use this if you want to manually decide when the filter needs to be called. You will get instance of the filters to the delegate and then can apply your custom logic for the specific filter. 

Example of this can be seen below:

```csharp
public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
{
    return Filters.CreateFor<Blog, BlogFilters>()
        //common filter definition, shouldRun is not used here but is resolved internally
        .Equal(blog => blog.Id, filter => filter.BlogId)
        //custom shouldRun delegate, this filter will not run even if filters are provided
        //due to this you can do some pretty spicy stuff by forcing filters to run or not run
        .In(blog => blog.Id, filter => filter.BlogIds, filters => false)
        .Build();
}
```
Due to nature of how `FilterProvider` implementation works you can even create a custom filter collection
which will depend on a specific parameter being passed in the request.

For example you could do something like next example (but you don't have to and I'm not sure why would you):

```csharp
public class BlogFiltersProvider : IFilterProvider<Blog, BlogFilters>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlogFiltersProvider(IHttpContextAccessor httpContextAccessor) 
        => _httpContextAccessor = httpContextAccessor;
    
    public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var hasSecretParam = !httpContext?.Request.Query.ContainsKey("secret") ?? true;
        
        return Filters.CreateFor<Blog, BlogFilters>()
            //this filter will now be used only when secret parameter is not preset in the URL
            //of the request
            .Equal(blog => blog.Id, filter => filter.BlogId, _ => hasSecretParam)
            .In(blog => blog.Id, filter => filter.BlogIds)
            .Build();
    }
}
```

Similar can be applied to `SorterProvider` implementations which also contain `shouldRun` parameter on
sorter builder extensions. Be careful when using `IHttpContextAccessor` in this way and be sure to read about
how to correctly use it over on [this link](https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AspNetCoreGuidance.md#do-not-store-ihttpcontextaccessorhttpcontext-in-a-field).

#### Automatic application in MVC via ResultFilter

If needed you can even streamline the whole process if you create a simple `IAsyncResultFilter` so that you only need to return `IQueryable<T>` instance back from your controller actions. 

Example can be seen below:

```csharp
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ooze.Typed.Paging;

namespace Ooze.Typed.Web.Filters;

public sealed class OozeFilter<TEntity, TFilters, TSorters> : IAsyncResultFilter
    where TEntity : class
{
    private readonly IOperationResolver<TEntity, TFilters, TSorters> _resolver;
    private readonly ILogger<OozeFilter<TEntity, TFilters, TSorters>> _log;

    public OozeFilter(
        IOperationResolver<TEntity, TFilters, TSorters> resolver,
        ILogger<OozeFilter<TEntity, TFilters, TSorters>> log)
    {
        _resolver = resolver;
        _log = log;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var objectResult = context.Result as ObjectResult;

        if (objectResult?.Value is IQueryable<TEntity> query
            && context.Controller is ControllerBase)
        {
            _log.LogDebug("Binding {modelName}", nameof(RequestInput));

            //notice that this implementation reads request body, which will be okay for non HTTP GET requests
            var model = await JsonSerializer.DeserializeAsync<RequestInput?>(context.HttpContext.Request.Body,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (model is not null)
            {
                objectResult.Value = _resolver.Apply(query, model.Sorters, model.Filters, model.Paging);
            }
            else
            {
                _log.LogWarning("Binding of {modelName} failed", nameof(RequestInput));
            }
        }

        await next();
    }

    private class RequestInput
    {
        public TFilters? Filters { get; set; }
        public IEnumerable<TSorters>? Sorters { get; set; }
        public PagingOptions? Paging { get; set; }
    }
}
```

This can in turn be used to decorate controller actions in order to minimize the whole process.

```csharp
[HttpPost("/sql-server-automatic"), ServiceFilter(typeof(OozeFilter<Blog, BlogFilters, BlogSorters>))]
public IQueryable<Blog> PostSqlServerAutomatic() => _sqlServerDb.Set<Blog>();
```

For more details look at sample project in `tests/Ooze.Typed.Web`.

#### Applying filters and sorters on IEnumerable collections

Due to nature of `IQueryable<T>` and `IEnumerable<T>` you can even use `OozeOperationResolver` on materialized or in memory collections for example `List<T>`. You'll just need to convert it (cast it) to `IQueryable<T>` via `.AsQueryable()` method. Notice that this can lead to exception since not all operations can be used this way. Some operations can't be used on `client side` and this can cause errors. 

Example of all this can be seen below:
```csharp
//BlogFiltersProvider - implementation can be seen in detail in sample project
.Like(blog => blog.Name, filter => filter.Name)

//BlogController
[HttpPost("/sql-server-automatic-enumerable"), ServiceFilter(typeof(OozeFilter<Blog, BlogFilters, BlogSorters>))]
public async Task<IQueryable<Blog>> PostSqlServerEnumerable()
{
    var results = await _sqlServerDb.Set<Blog>()
        .ToListAsync();

    return results.AsQueryable();
}
```
As mentioned before if you use `.Like()` filter by providing `Name` filter you'll encounter an exception as noted before. 
```
 System.InvalidOperationException: The 'Like' method is not supported because the query has switched to client-evaluation. This usually happens when the arguments to the method cannot be translated to server. Rewrite the query to avoid client evaluation of arguments so that method can be translated to server.
```

Be sure to know what you'll be using this for so you don't get unwanted issues.

### Filter extensions
As previously mentioned additional packages contains some usefull extensions when working with specific "flavor" of EF. For example you might be using `Sqlite` or `SqlServer` or `Postgres` etc. For these situations you can install these specific packages which contain extensions methods for the specific flavor. More about what is supported on each of the packages can be seen below.

#### [Ooze.Typed.EntityFrameworkCore](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore/)
This packages depends on EF Core packages and exposes next extensions:
 - `Like()` - EF.Eunctions.Like

#### [Ooze.Typed.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Sqlite/)
This packages depends on EF Core Sqlite package and package mentioned beforehand and exposes next extensions:
 - `Glob()` - EF.Functions.Glob

#### [Ooze.Typed.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.SqlServer/)
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

#### [Ooze.Typed.EntityFrameworkCore.Npgsql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.Npgsql)
This package depends on EF Core Npgsql package and package mentioned beforehand and exposes next extensions:
 - `InsensitiveLike()` - EF.Functions.ILike
 - `SoundexEqual()` - EF.Functions.FuzzyStringMatchSoundex

#### [Ooze.Typed.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Ooze.Typed.EntityFrameworkCore.MySql)
This package depends on EF Core MySql (Pomelo) package and package mentioned beforehand and exposes next extensions:
- `IsDateDiffDay()` - EF.Functions.DateDiffDay
- `IsDateDiffMonth()` - EF.Functions.DateDiffMonth
- `IsDateDiffYear()` - EF.Functions.DateDiffYear
- `IsDateDiffHour()` - EF.Functions.DateDiffHour
- `IsDateDiffMinute()` - EF.Functions.DateDiffMinute
- `IsDateDiffSecond()` - EF.Functions.DateDiffSecond
- `IsDateDiffMicrosecond()` - EF.Functions.DateDiffMicrosecond
 