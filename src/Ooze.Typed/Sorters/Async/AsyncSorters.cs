namespace Ooze.Typed.Sorters.Async;

public static class AsyncSorters
{
    public static IAsyncSorterBuilder<TEntity, TSorters> CreateFor<TEntity, TSorters>()
        => new AsyncSorterBuilder<TEntity, TSorters>();
}