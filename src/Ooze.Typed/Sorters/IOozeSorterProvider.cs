namespace Ooze.Typed.Sorters
{
    public interface IOozeSorterProvider<TEntity>
    {
        IEnumerable<ISortDefinition<TEntity>> GetSorters();
    }
}
