namespace Ooze.Typed.Sorters.Async;

public interface IAsyncSorterProvider<TEntity, TSorters>
{
    ValueTask<IEnumerable<AsyncSortDefinition<TEntity, TSorters>>> GetSortersAsync();
}