## Sorters

Sorters can be created by implementing an registering and implementation of `ISorterProvider` interface to the service collection. They will provide you an ability to apply sorting to different `IQueryable` instances.

## Table of Contents
- [Creating a sorter provider](#creating-a-sorter-provider)
- [Registering sorter implementation](#registering-sorter-implementation)
- [Async support](#async-support)
- [Advanced](#advanced)

## Creating a sorter provider
Sorter provider can be created by implementing `ISorterProvider` interface. Interface itself requires 2 generic type arguments. First one denotes the `type of IQueryable query` while the second one an sorter holder type. Sorter holder type will contain data that will be used for sorting of `IQueryable` queries based of allowed sorters. Lets explore an simple example below:

```csharp
public class Blog 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BlogSorters
{
    public SortDirection? Id { get; set; }
    public SortDirection? Date { get; set; }
}


public class BlogSortersProvider : ISorterProvider<Blog, BlogSorters>
{
    public IEnumerable<SortDefinition<Blog, BlogSorters>> GetSorters()
    {
        return Sorters.CreateFor<Blog, BlogSorters>()
            .SortBy(blog => blog.Id, sorter => sorter.Id)
            .SortBy(blog => blog.CreatedAt, sorter => sorter.Date)
            .Build();
    }
}

```

Notice the usage of `SortDirection` enum which will tell us in which direction we want to sort the `IQueryable` query. Using `SortDirection` enum is required in order to implement sorter providers correctly.

Additionally since `.SortBy` method requires a generic expression for sorting you can target navigational properties also in order to create more complex sorting scenarios.

## Registering sorter implementation
In order for `Ooze` to see your sorter implementations you will need to register it into the `ServiceCollection`. Easiest way to do so is to use `.Add()` method that is exposed on the builder that is returned by calling `.AddOozeTyped()`. You can also register it manually to the `ServiceCollection` if needed.

By default all provider implementations registered via `Add` method are `Singletons`. But you can override this behaviour by passing lifetime as additional argument to the `Add` method.

## Async support
Sorters, similar to filters, can also be used in `async` fashion. Async support is currently hidden behind `EnableAsyncResolvers` call. If you wish to use async sorters be sure to call that while registering related `Ooze` code. Below we can see example of async sorter provider declaration, notice the usage of `IAsyncSorterProvider` and `AsyncSorters`.

```csharp
public class BlogSortersProvider : IAsyncSorterProvider<Blog, BlogSorters>
{
    public ValueTask<IEnumerable<AsyncSortDefinition<Blog, BlogSorters>>> GetSortersAsync()
    {
        return ValueTask.FromResult(AsyncSorters.CreateFor<Blog, BlogSorters>()
            .SortBy(blog => blog.Id, sort => sort.BlogIdSort)
            .SortBy(blog => blog.Name, sort => sort.BlogNameSort, _ => false)
            .SortBy(blog => blog.Posts.Count, sort => sort.PostsCountSort)
            .Build());
    }
}
```

`IAsyncSorterProvider` is a sorter provider interface that works in similar fashion to non async `ISorterProvider`. Same goes for `AsyncSorters` builder vs `Sorters` builder. All sorting operations that are available on the non async version are also exposed on async supported one. In order to consume these sorters you will also need to register them to `service collection` and then use `IAsyncOperationResolver` in order for them to be applied.

In current versions `Cancellation tokens` are not yet supported.

## Advanced
Sorters providers also support `shouldRun` parameter similar to how filters do. Other than that most of the advanced scenarios covered under [Advanced](./filters.md#advanced) section of filter documentation also cover sorter implementations.