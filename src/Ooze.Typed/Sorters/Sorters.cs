namespace Ooze.Typed.Sorters
{
    public static class Sorters
    {
        public static ISorterBuilder<TEntity> CreateFor<TEntity>()
            => new SorterBuilder<TEntity>();
    }
}
