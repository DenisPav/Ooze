## Query language
`Ooze.Typed.Query` package contains implementation of query language for Ooze. This will enable you to write `human readable` queries in a similar manner that you might have seen in products like Jira.


## Table of Contents
- [Creating a query filter provider](#creating-a-query-filter-provider)
- [Filter operations](#filter-operations)
- [Registering query filter implementation](#registering-query-filter-implementation)
- [Usage](#usage)


## Creating a query filter provider
Query filter provider can be created by implementing `IQueryLanguageFilterProvider` interface. Interface itself requires a generic type argument. It denotes the `type of IQueryable query`. Lets explore an simple example below:

```csharp
public class BlogQueryFiltersProvider :  IQueryLanguageFilterProvider<Blog>
{
    public IEnumerable<QueryLanguageFilterDefinition<Blog>> GetMappings()
    {
        return QueryLanguageFilters.CreateFor<Blog>()
            .Add(x => x.Name, "newName")
            .Build();
    }
}
```

`BlogQueryFiltersProvider` class will hold different available filters that we can use in our query language. We are using `QueryLanguageFilters` static class to create a collection of filters for `Blog` entity from previous examples under [filters](filters.md) page. `.Add` method on the builder will by default use targeted property name as the name in the query language query, but it can be overriden via a second parameter if you want to rename it (`newName` from previous example). 

## Filter operations
Operations that are available in the query language can be seen below together with their related tokens in the query:
 - Greater than: >>;
 - Greater than Or Equal: >=;
 - Less than: <<;
 - Less than or equal: <=;
 - Equal to: ==;
 - Not equal to: !=;
 - Starts with: @=;
 - Ends with: =@;
 - Contains: %%;

## Registering query filter implementation
In addition to the initial service registration as explained in previous documentation, for query language package to work you'll also need to call `AddOozeQueryLanguage()`. This call will expose `.AddQueryProvider()` method which you can use to register your query filter provider implementations.

## Usage
In order to use query language with other Ooze features be sure to inject `IQueryLanguageOperationResolver` into your code. This interface will expose `FilterWithQueryLanguage` methods which can then be used to parse and apply filters from your queries. Example can be seen below:

```csharp
[HttpGet]
public async Task<IEnumerable<Blog>> FilterBlogs(
    [FromServices] DatabaseContext db,
    [FromServices] IQueryLanguageOperationResolver resolver,
    CancellationToken cancellationToken,
    string query)
{
    var queryable = db.Set<Blog>();
    queryable = resolver.FilterWithQueryLanguage(queryable, query)

    return await queryable.ToListAsync(cancellationToken);
}
```