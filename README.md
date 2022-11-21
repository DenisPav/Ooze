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
by `Ooze` to sort your queries. This is done by implementing `IOozeSorterProvider<TEntity>` interface, and using `Sorters` static class to start of builder for creating sorters. Example of this can be found below:
```csharp
public class MyClassSortersProvider : IOozeSorterProvider<MyClass>
{
    public IEnumerable<ISortDefinition<MyClass>> GetSorters()
    {
        return Sorters.CreateFor<MyClass>()
            //add sorting on Id property in provided direction
            .Add(x => x.Id)
            .Build();
    }
}
```
Sorters are added to `Ooze` in same manner as Filters so you can reuse the example mentioned there.

**NOTE:**
Sorters by default  `Sorters` record class which uses combination of name and `SortDirection` enumeration in order to specify property sorting direction.

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

## Ooze.Typed.Query 🔎
Additional package for `Ooze.Typed` which enables `Queries`. This is a concept where you can write actual
expression as a string representation for filtering. For example if you have bunch of `Posts` which have `Id` and `Name` columns, you could filter them out via something like `Id >= '3' && Name != 'My Post'`. This in turn will be translated into the correct expression and applied to your `IQueryable` instance.

Supported logical operations for queries are AND (`&&`) and OR (`||`). On the other side of things supported value
operators are next:
 * GreaterThan - `>>`
 * GreaterThanOrEqual -`>=`
 * LessThan - `<<`
 * LessThanOrEqual - `<=`
 * Equal - `==`
 * NotEqual - `!=`

In order for this to work correctly you'll need to add implementation of `IQueryFilterProvider<T>` which will need
to be registered to `ServiceCollection`. This implementation will contain filter fields which you enable for use in queries. Example implementation can be seen below (based on previous `Blog` example):

```csharp
public class BlogQueryFiltersProvider : IOozeQueryFilterProvider<Blog>
{
    public IEnumerable<IQueryFilterDefinition<Blog>> GetFilters()
    {
        return QueryFilters.CreateFor<Blog>()
            .Add(x => x.Id, name: "Id")
            .Add(x => x.Name, name: "Name")
            .Build();
    }
}
```

<details>
<summary>Old Version (not typed) in this dropdown</summary>
<br>

# 🌳💧 Ooze - Sorting, Filtering, Paging and Selections for ASP.NET Core and EF Core

## ⚙ Setup
You'll need to add reference to the package (most easily by adding it via `git modules` to the project). After that call `.AddOoze()` method on services inside of `ConfigureServices()` method in your startup class.

Create a configuration class for your EF entity class. For example:
```csharp
// sample EF entity class
public class Post
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
}

// sample Ooze configuration class
public class PostConfiguration : IOozeConfiguration
{
    public void Configure(IOozeConfigurationBuilder builder)
    {
        builder.Entity<Post>()
            // we can now sort by Enabled column
            .Sort(post => post.Enabled)
            // and filter by Id column
            .Filter(post => post.Id);
    }
}
```

Next step is to inject `IOozeResolver` into your controller, create instance of `OozeModel` (via modelbinding or manually) and pass it to `.Apply()` method of
resolver. That's it, after that just materialize query comming from `IOozeResolver` and deal with data. Example of this can be seen below:
```csharp
[Route("[controller]")]
public class TestController : ControllerBase
{
    readonly DatabaseContext _db;
    readonly IOozeResolver _resolver;

    public TestController(
        DatabaseContext db,
        IOozeResolver resolver)
    {
        _db = db;
        // inject instance of IOozeResolver
        _resolver = resolver;
    }

    [HttpGet]
    public IActionResult Get([FromQuery]OozeModel model)
    {
        // get IQueryable instance from EF DbContext
        IQueryable<Post> query = _db.Posts;
        // apply sorting and filters to IQueryable
        query = _resolver.Apply(query, model);

        return Ok(query.ToList());
    }
}
```

## ↕ Sorting
Sorting can be done by just specifying name of the sorter that was done via configuration. Order can be inversed by using `-` operator before actual sorter name. Example can be seen below:
```csharp
//ascending
"?sorters=id"

//descending
"?sorters=-id"
```

Non existing sorters will be skipped in runtime, only correct ones will be aplied.

## 🗡🧀 Filtering
Filtering can be done by specifying `filter` which is then followed by an `operation` which is then followed by `value` (`filter`->`operation`->`value`). Example of filtering can be seen below:
```csharp
//filter by id filter where each value is higher then number 2
?filters=id>2
```

Supported operations are next:

* Equal - `==`,
* Not Equal - `!=`,
* Greater Than Or Equal - `>=`,
* Less Than Or Equal - `<=`
* Starts With - `@=`
* Ends With - `=@`
* Greater Than - `>`
* Less Than - `<`
* Contains - `@`

Operations need to be unique or otherwise validation error will occur, also only symbols can be used for them. Non existing filters will be skipped in runtime, only correct ones will be aplied.

## 📄 Paging
Paging can be enabled if you switch the flag in configuration of `.AddOoze` method. After turning paging on, arguments form `OozeModel` will be consumed and applied to `IQueryable<>` instance. You can configure default `page size` in the configuration lambda also. Sample of this can be seen below:
```csharp
//ConfigureServices() method in Startup.cs
services.AddOoze(typeof(Startup).Assembly, opts =>
{
    //enable paging
    opts.Paging.UsePaging = true;
    //change default pagesize from 20 to 88
    opts.Paging.DefaultPageSize = 88;
});
```
By default internal implementation of `PagingHandler` will use exact number as specified by `DefaultPageSize` or `PageSize` from `OozeModel` if it was passed in request. If you need to know if there are more pages, you can create your own implementation and replace existing registration in `ServiceCollection`. More about that can be seen below under `Advanced configuration`. More info can be seen in sample in `Tests/Ooze.Web`.

## 🗡 Selections
You can also let Ooze cut the total selections that go out of the `IQueryable<>` instance. In order to enable that you need to switch the flag in configurator of `AddOoze()` method (You don't need to have configuration classes for Selections to work). Example of that can be seen below:
```csharp
//ConfigureServices() method in Startup.cs
services.AddOoze(typeof(Startup).Assembly, opts => opts.UseSelections = true);
```
After that everything that goes through `IOozeResolver` will be handled and cutted if not present in `Fields` property of `OozeModel`. More detailed example of that can be seen below:
```csharp
//sample EF Model
public class Post
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }

    public ICollection<Comment> Comments { get; set; }
}

public class Comments
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }
}

public class SampleController : ControllerBase
{
    readonly IOozeResolver _resolver;
    //example DbContext instance
    readonly DatabaseContext _db;

    public SampleController(
        IOozeResolver resolver,
        DatabaseContext db)
    {
        _resolver = resolver;
        _db = db;
    }

    //you can bind values from query string, body... anything you want
    [HttpGet]
    public async Task<IActionResult> Sample([FromQuery]OozeModel model)
    {
        IQueryable<Post> query = Db.Set<Post>();

        //if query string contains for example fields=id,name
        //we only select id,name from DB
        return await _resolver.Apply(query, model)
            .ToListAsync();
    }
}
```
```sql
--SQL that might get executed for this is next:
SELECT "p"."Id", "p"."Name"
FROM "Posts" AS "p"

--Without selections this might look like following:
SELECT "p"."Id", "p"."Date", "p"."Name"
FROM "Posts" AS "p"
```

Similar to upper example if you `OozeModel` gets values like `Name,Comments.Text` you'll get something in terms of next SQL:
```sql
SELECT "p"."Name", "p"."Id", "c"."Text", "c"."Id"
FROM "Posts" AS "p"
LEFT JOIN "Comments" AS "c" ON "p"."Id" = "c"."PostId"
ORDER BY "p"."Id", "c"."Id"
```
without selections that would be more like
```sql
SELECT "p"."Id", "c"."Id", "c"."Date", "c"."PostId", "c"."Text"
FROM "Posts" AS "p"
LEFT JOIN "Comments" AS "c" ON "p"."Id" = "c"."PostId"
ORDER BY "p"."Id", "c"."Id"
```

## 🧪 Queries
Queries are a bit different feature which enables you to write readable `Filters`. Also you can combine them with logical filters (`AND`, `OR`) in order to create more complex samples. Queries use `Filter` configuration as a source while translating. Queries also take precedence over `Sorters` and `Filters`. Example of queries can be seen below:
```csharp
//where id is 3 and name isn't something
?query= id == 3 AND name != 'something'
```
NOTE: custom filters aren't usable with queries

## ⚒ Advanced configuration
### Operations
Currently you can configure operation strings via parameter in `.AddOoze()` extension method. There is a restriction to that though. You can use only symbols and not letters or numbers. Example of that can be seen below:
```csharp
//register Ooze services
services.AddOoze(typeof(Startup).Assembly, opts => 
{
    //change greater than operation string mapping
    opts.Operations.GreaterThan = "."
});
```
### Custom filtering
You can extend Ooze default operations by creating implementation of `IOozeFilterProvider<TEntity>` and then registering it to container. Example can be seen below:
```csharp
//create implementation of IOozeFilterProvider<>
public class CustomFilterProvider : IOozeFilterProvider<Post>
{
    //specify name under which filter will be triggered
    public string Name => "custom";

    public IQueryable<Post> ApplyFilter(IQueryable<Post> query, FilterParserResult filter)
    {
        //filter passed IQueryable instance
    }
}

//Startup.cs
//register to container with wanted lifetime
services.AddScoped<IOozeProvider, CustomFilterProvider>();
```

### Custom sorting
Similar to filtering, sorting can be also extended by creating implementation of `IOozeSorterProvider<TEntity>`. Example can be seen below:
```csharp
//create implementation of IOozeSorterProvider<>
public class CustomSorterProvider : IOozeSorterProvider<Post>
{
    //specify name under which sorter will be triggered
    public string Name => "custom";

    public IQueryable<Post> ApplySorter(IQueryable<Post> query, bool ascending)
    {
        //sort IQueryable
    }

    public IOrderedQueryable<Post> ThenApplySorter(IOrderedQueryable<Post> query, bool ascending)
    {
        //if some sorting was already done this method will get triggered
        //and you'll get presorted IQueryable on which you can then use
        //ThenBy methods
    }
}

//Startup.cs
//register to container with wanted lifetime
services.AddScoped<IOozeProvider, CustomSorterProvider>();
```

NOTE: If you're using custom sorter or filter provider you need to name it uniquely. Otherwise you'll get exceptions while doing operations with them. Handlers are based on implementations for specific field sorter, filter that comes from builder so if you create a custom one which has the same name, code won't be able which one to use and due to that will throw an exception in runtime.

### Custom paging
Similar to above sample, you can create a custom paging handler which will do your logic on related queryable. For example you might want to pull 1 additional record from query in order to determine if there is a next page available, or something similar. To do so you need to create implementation of `IOozePagingHandler`. Example of this can be seen below (more detailed example is in `tests/Ooze.Web` sample):
```csharp
//custom paging handler
public class CustomPagingProvider : IOozePagingHandler
{
    public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, int? page, int? pageSize)
    {
        //paging logic
    }
}

//alter ServiceCollection registrations in ConfigureServices() method
//NOTE: this goes after .AddOoze() call

//remove old registration
services.Remove(services.First(descriptor => descriptor.ServiceType.Equals(typeof(IOozePagingHandler))));
//add new registration
services.Add(new ServiceDescriptor(typeof(IOozePagingHandler), typeof(CustomPagingProvider), ServiceLifetime.Scoped));
```

## 📖 Logging
There is simple debug logging in most of the DI enabled code. If you want to enable it be sure to change default log level to `Debug` or change log level for Ooze. Sample of this is below:
```json
{
    "Logging": {
        "LogLevel": {
            "Ooze": "Debug"
        }
    },
}
```


## ⛓ MVC / Controllers
`Ooze.AspNetCore` package provides a [Result filter](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#result-filters) which can abstract boilerplate code for you. You just need to anotate action on which you want to use `Ooze` and that's it (or you can apply it globally).
Example of this can be seen below:
```csharp
//Startup.cs

//register Ooze
services.AddOoze(typeof(Startup).Assembly);
//register OozeFilter
services.AddScoped(typeof(OozeFilter<>));

//TestController.cs

[HttpGet("query")]
//we register Ooze for this action (action should return IQueryable instance)
[ServiceFilter(typeof(OozeFilter<Post>))]
public IQueryable<Post> GetQuery() => _db.Posts;
```
</details>