namespace Ooze.Typed.Sorters;

public static class Sorters
{
    public static ISorterBuilder<TEntity, TSorters> CreateFor<TEntity, TSorters>()
        => new SorterBuilder<TEntity, TSorters>();
}
