namespace Ooze.Typed.Filters;

public static class Filters
{
    public static IFilterBuilder<TEntity, TFilter> CreateFor<TEntity, TFilter>()
        => new FilterBuilder<TEntity, TFilter>();
}
