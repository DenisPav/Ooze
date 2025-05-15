## Filters

Filters can be created by implementing an registering and implementation of `IFilterProvider` interface to the service collection. They will provide you an ability to apply filters to different `IQueryable` instances.

## Table of Contents
- [Creating a filter provider](#creating-a-filter-provider)
- [Filter operations](#filter-operations)
- [Registering filter implementation](#registering-filter-implementation)
- [Async support](#async-support)
- [Advanced](#advanced)
  - [shouldRun parameter](#shouldrun-parameter)
  - [Filter provider lifetimes](#filter-provider-lifetimes)
  - [Filtering in-memory data](#filtering-in-memory-data)
  

## Creating a filter provider
Filter provider can be created by implementing `IFilterProvider` interface. Interface itself requires 2 generic type arguments. First one denotes the `type of IQueryable query` while the second one an filter holder type. Filter holder type will contain data that will be used for filtering of `IQueryable` queries based of allowed filters. Lets explore an simple example below:

```csharp
public class Blog 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BlogFilters
{
    public int[] Ids { get; set; }
    public string? Name { get; set; }
}


public class BlogFiltersProvider : IFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            .In(blog => blog.Id, filter => filter.BlogIds)
            .Like(blog => blog.Name, filter => filter.Name)
            .Build();
    }
}

```

In the previous example we can see that we have a simple `Blog` class which will be used as our `entity` class. `BlogFilters` class will hold different available filters that we can use. `BlogFiltersProvider` contains implementation of filtering for previously mentioned two types (classes). So if we provide `Ids` in this case while using filtering, related `IQueryable<Blog>` instance will be checked to see if the blog id is contained in the `Ids` array. Similar to that, if we provide `Name` in the `BlogFilters` class it will be used to call `EF.Functions.Like` over the blog name in order to filter the ones that are matching the expression.

## Filter operations
Some common filter operations are available while using `Filters` builder to create filtering definitions. We can see all of them below:
 - Equal - check if property is equal to filter
 - NotEqual - check if property is different than filter
 - GreaterThan - check if property is greater than filter
 - LessThan - check if property is less than filter
 - In - check if property is contained in filter array
 - NotIn - check if property is not contained in filter array
 - Range - check if property falls in range of filter
 - OutOfRange - check if property falls outside of range of filter
 - StartsWith - check if string property starts with filter
 - DoesntStartWith - check if string property doesn't starts with filter
 - EndsWith - check if string property ends with filter
 - DoesntEndWith - check if string property doesn't end with filter
 - Add - creates a custom filter definition

`Add` method on the `FilterBuilder` is particullary powerfull as it allows us to create fully custom filter definitions. For example we could combine 2 filters into a single one or check something addtitionally before running a filter. Lets reuse the example classes from the initial example and add a custom filter in this case:

```csharp
public class BlogFiltersProvider : IFilterProvider<Blog, BlogFilters>
{
    public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        return Filters.CreateFor<Blog, BlogFilters>()
            .Add(filter => filter.Ids != null && filter.Name != null,
            filter => blog => blog.Name == "combined filter")
            .Build();
    }
}
```

Here we are registering custom filter, which will run in case when both `Ids` and `Name` are provided. In that case it will filter `IQueryable<Blog>` to find a blog with `Name` which is equal to `combined filter` string. 

Since `Add` method receives a factory which creates an `expression` that is applied to `IQueryable` instance you can even open the body of it. And thus to additional logic in it if needed. Consider the next example for reference:

```csharp
.Add(filter => filter.Ids != null, filter => 
{
    var uniqueIds = filter.Ids.Distinct();

    return blog => uniqueIds.Contains(blog.Id);
})
```

## Registering filter implementation
In order for `Ooze` to see your filter implementation you will need to register it into the `ServiceCollection`. Easiest way to do so is to use `.Add()` method that is exposed on the builder that is returned by calling `.AddOozeTyped()`. You can also register it manually to the `ServiceCollection` if needed.

By default filter implementations registered via `Add` method are `Singletons`. But you can override this behaviour by passing lifetime as additional argument to the `Add` method.

## Async support
Filters can also be used in `async` fashion. Async support is currently hidden behind `EnableAsyncResolvers` call. If you wish to use async filtering be sure to call that while registering related `Ooze` code. Below we can see example of async filter declaration, notice the usage of `IAsyncFilterProvider` and `AsyncFilters`.

```csharp
public class BlogFiltersProvider : IAsyncFilterProvider<Blog, BlogFilters>
{
     public ValueTask<IEnumerable<AsyncFilterDefinition<Blog, BlogFilters>>> GetFiltersAsync()
        => ValueTask.FromResult(AsyncFilters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Name, filter => filter.Name)
            .AddAsync(async filters =>
            {
                await Task.CompletedTask;
                return filters.Ids != null;
            }, async filters =>
            {
                await Task.CompletedTask;
                return blog => filter.Ids.Contains(blog.Id);
            })
            .Build());
}
```

`IAsyncFilterProvider` is a filter provider interface that works in similar fashion to non async `IFilterProvider`. Same goes for `AsyncFilters` builder vs `Filters` builder. All filter operations that are available on the non async version are also exposed on async supported one. In order to consume these filters you will also need to register them to `service collection` and then use `IAsyncOperationResolver` in order for them to be applied.

In current versions `Cancellation tokens` are not yet supported.

## Advanced

### shouldRun parameter
This parameter is present in all filter operations. If you do not provide it it will be defaulted to a sane default. With it you can decide when you want to allow some filter to be applied or to be skipped. Example can be seen below:

```csharp
return Filters.CreateFor<Blog, BlogFilters>()
        .Equal(blog => blog.Name, filter => filter.Name, filter => filter.Name?.StartsWith("start"))
        .In(blog => blog.Id, filter => filter.Ids, filters => false)
        .Build();
```

In this example `equality` filter will be applied in case when filter argument will contain string `start`. While `contains/in` filter will never be applied.

### Filter provider lifetimes
By default when you register a `Filter provider` via `.Add()` method, it will be registered as `Singleton`. This can be changed to `Scoped/Transient` if needed. Due to this you can even create filters differently depending on some external factors. Check the examples below:

```csharp
//register this filter provider with .Add<BlogFiltersProvider>(ServiceLifetime.Scoped)
public class BlogFiltersProvider : IFilterProvider<Blog, BlogFilters>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BlogFiltersProvider(IHttpContextAccessor httpContextAccessor) 
        => _httpContextAccessor = httpContextAccessor;
    
    public IEnumerable<FilterDefinition<Blog, BlogFilters>> GetFilters()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var hasSecretParam = !httpContext?.Request.Query.ContainsKey("secret") ?? false;
        
        return Filters.CreateFor<Blog, BlogFilters>()
            .Equal(blog => blog.Name, filter => filter.Name, _ => hasSecretParam)
            .Build();
    }
}
```

Here we can see that `equality` filter will only be applied in case when `query string` will contain `secret` parameter. Otherwise it will be skipped. In order for this to work correctly be sure to change default `lifetime` of a filter provider upon registration.

### Filtering in-memory data
Due to nature of `IQueryable/IEnumerable` collections, you can apply filter providers even on a `List` if needed. This does come with some limitations also. For example `.Like` operation would not work and would throw `System.InvalidOperationException` in that case. Like operator is used as a marker in case of `IQueryable` queries which gets transformed into the `like` sql counterpart and thus in case of `IEnumerable` is not present. On the other hand, common in-memory operations will continue to work normally as before. Example of this can be seen below:

```csharp
[HttpPost]
public async Task<IEnumerable<Blog>> FilterBlogs(
    [FromService] DatabaseContext db,
    [FromService] IOperationResolver resolver
    BlogFilters filters)
{
    var materializedBlogs = await db.Set<Blog>()
        .ToListAsync();
    var blogsCastedToQueryable = materializedBlogs.AsQueryable();
    blogsCastedToQueryable = resolver.Filter(blogsCastedToQueryable, Filters);

    return blogsCastedToQueryable.ToList();
}
```