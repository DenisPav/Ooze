using System.Linq.Expressions;

namespace Ooze.Typed.Query.Filters;

internal class QueryFilterBuilder<TEntity> : IQueryFilterBuilder<TEntity>
{
    readonly IList<QueryFilterDefinition<TEntity>> _filterDefinitions = new List<QueryFilterDefinition<TEntity>>();

    public IQueryFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string name = null)
    {
        _filterDefinitions.Add(new QueryFilterDefinition<TEntity>
        {
            //TODO: extract this from actual expression if name is not passed
            Name = name ?? throw new ArgumentException(nameof(name)),
            DataExpression = dataExpression
        });

        return this;
    }

    public IEnumerable<IQueryFilterDefinition<TEntity>> Build()
    {
        return _filterDefinitions;
    }
}