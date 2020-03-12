# 🌳💧 Ooze - Sorting and Filtering for ASP.NET Core

## ⚙ Setup
You'll need to add reference to the package (insert link here when it will be available). After that call `.AddOoze()` method on services inside of `ConfigureServices()` method in your startup class.

Create a configuration class for your EF entity class. For example:
```cs
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
```cs
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
```cs
//ascending
"?sorters=id"

//descending
"?sorters=-id"
```

## 🗡🧀 Filtering
Filtering can be done by specifying `filter` which is then followed by an `operation` which is then followed by `value` (`filter`->`operation`->`value`). Example of filtering can be seen below:
```cs
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