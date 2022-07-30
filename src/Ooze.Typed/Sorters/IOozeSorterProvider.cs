namespace Ooze.Typed.Sorters
{
    public interface IOozeSorterProvider<TEntity, TSorters>
    {
        IEnumerable<ISortDefinition<TEntity, TSorters>> GetSorters();
    }
}
