## Paging

Paging is yet another common piece for api operations. Ooze contains implementations of `skip-take` and `cursor` paging.

## Table of Contents
- [Skip take paging](#skip-take-paging)
- [Cursor paging](#cursor-paging)

## Skip take paging
Paging is done via `.Page` method on resolver interface. You just need to pass instance of `PagingOptions` to the before mentioned method and combine it with your `IQueryable<T>` instance. For example:

```csharp
query = resolver.Page(query, model.Paging);
```

## Cursor paging
There is also additional support for `cursor paging` via `.PageWithCursor` method on the resolver interface. If you want to use cursor paging you'll be using `CursorPagingOptions<T>` in this case and you'll need to pass a property you want to use as a cursor source. Example of this can be found in the next example:

```csharp
query = nonTypedResolver.PageWithCursor(
    query, 
    entity => entity.Id, 
    new CursorPagingOptions<int>
    {
        After = 0,
        Size = 3,
    });
```