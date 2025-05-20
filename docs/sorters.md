## Sorters

Sorters can be created by implementing an registering and implementation of `ISorterProvider` interface to the service collection. They will provide you an ability to apply sorting to different `IQueryable` instances.

## Table of Contents
- [Creating a sorter provider](#creating-a-sorter-provider)

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