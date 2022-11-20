using Ooze.Typed.Queries;
using Ooze.Typed.Query.Filters;

namespace Ooze.Typed.Query;

internal class OozeQueryHandler<TEntity> : IOozeQueryHandler<TEntity>
{
    private readonly IEnumerable<IOozeQueryFilterProvider<TEntity>> _filterProviders;

    public OozeQueryHandler(IEnumerable<IOozeQueryFilterProvider<TEntity>> filterProviders)
    {
        _filterProviders = filterProviders;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition)
    {
        var filterDefinitions = _filterProviders.SelectMany(provider => provider.GetFilters())
            .Cast<QueryFilterDefinition<TEntity>>()
            .ToList();
        
        var result = QueryTokenizer.Tokenize(filterDefinitions, queryDefinition);
        var expressionResult = QueryExpressionCreator.Create<TEntity>(query.Expression, result);
        if (expressionResult.Error != null)
            throw expressionResult.Error;
        
        return query.Provider.CreateQuery<TEntity>(expressionResult.Expression);
    }
}