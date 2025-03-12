using Microsoft.Extensions.Logging;
using Ooze.Typed.Query.Expressions;
using Ooze.Typed.Query.Filters;
using Ooze.Typed.Query.Tokenization;

namespace Ooze.Typed.Query;

internal class QueryHandler<TEntity>(
    IEnumerable<IQueryFilterProvider<TEntity>> filterProviders,
    ILogger<QueryHandler<TEntity>> log)
    : IQueryHandler<TEntity>
{
    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        string queryDefinition)
    {
        log.LogDebug("Starting expression translation for query: [{query}]", queryDefinition);
        var filterDefinitions = filterProviders.SelectMany(provider => provider.GetMappings())
            .ToArray();

        var result = QueryTokenizer.Tokenize(filterDefinitions, queryDefinition);
        log.LogDebug("Query tokenization finished.");
        var expressionResult = QueryExpressionCreator.Create(filterDefinitions, query.Expression, result);
        if (expressionResult.Error != null)
            throw expressionResult.Error;

        log.LogDebug("Created expression: [{expression}].", expressionResult.Expression);
        return query.Provider.CreateQuery<TEntity>(expressionResult.Expression!);
    }
}