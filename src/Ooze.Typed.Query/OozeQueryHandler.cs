using Microsoft.Extensions.Logging;
using Ooze.Typed.Query.Expressions;
using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tokenization;

namespace Ooze.Typed.Query;

internal interface IOozeQueryHandler<TEntity>
{
    IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition);
}

internal class OozeQueryHandler<TEntity> : IOozeQueryHandler<TEntity>
{
    private readonly IEnumerable<IOozeQueryFilterProvider<TEntity>> _filterProviders;
    private readonly ILogger<OozeQueryHandler<TEntity>> _log;

    public OozeQueryHandler(
        IEnumerable<IOozeQueryFilterProvider<TEntity>> filterProviders,
        ILogger<OozeQueryHandler<TEntity>> log)
    {
        _filterProviders = filterProviders;
        _log = log;
    }

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition)
    {
        _log.LogDebug("Starting expression translation for query: [{query}]", queryDefinition);
        var filterDefinitions = _filterProviders.SelectMany(provider => provider.GetFilters())
            .Cast<QueryFilterDefinition<TEntity>>()
            .ToList();

        var result = QueryTokenizer.Tokenize(filterDefinitions, queryDefinition);
        _log.LogDebug("Query tokenization finished.");
        var expressionResult = QueryExpressionCreator.Create<TEntity>(filterDefinitions, query.Expression, result);
        if (expressionResult.Error != null)
            throw expressionResult.Error;

        _log.LogDebug("Created expression: [{expression}].", expressionResult.Expression);
        return query.Provider.CreateQuery<TEntity>(expressionResult.Expression);
    }
}