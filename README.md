# 🌳💧 Ooze - Sorting, Filtering and Selections for ASP.NET Core and EF Core

## ⚙ Setup
You'll need to add reference to the package (insert link here when it will be available). After that call `.AddOoze()` method on services inside of `ConfigureServices()` method in your startup class.

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

## 🗡 Selections
You can also let Ooze cut the total selections that go out of the `IQueryable<>` instance. In order to enable that you need to switch the flag in configurator of `AddOoze()` method. Example of that can be seen below:
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

## 🧪 Queries
Queries are a bit different feature which enables you to write readable `Filters`. Also you can combine them with logical filters (`AND`, `OR`) in order to create more complex samples. Queries use `Filter` configuration as a source while translating. Queries also take precedence over `Sorters` and `Filters`. Example of queries can be seen below:
```csharp
//where id is 3 and name isn't something
?query= id == 3 AND name != 'something'
```
NOTE: custom filters aren't usable with queries

## ⚒ Configuration
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