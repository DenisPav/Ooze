using Microsoft.Extensions.Logging;
using Ooze.Expressions;
using Ooze.Parsers;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Selections
{
    internal class OozeSelectionHandler : IOozeSelectionHandler
    {
        const char _fieldDelimiter = '.';

        readonly ILogger<OozeSelectionHandler> _log;

        public OozeSelectionHandler(ILogger<OozeSelectionHandler> log) => _log = log;

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string fields)
            where TEntity : class
        {
            _log.LogDebug("Running selection IQueryable changes");

            var results = GetParsedSelections(fields);
            var paramExpr = Parameter(typeof(TEntity), typeof(TEntity).Name.ToLower());
            var assignments = OozeExpressionCreator.CreateAssignments(paramExpr, typeof(TEntity), results);

            if (OozeExpressionCreator.LambdaExpr(typeof(TEntity), paramExpr, assignments) is Expression<Func<TEntity, TEntity>> lambda)
            {
                query = query.Select(lambda);
            }

            _log.LogDebug("Final selection expression: {expression}", query.Expression.ToString());
            return query;
        }

        IEnumerable<FieldDefinition> GetParsedSelections(string fields)
        {
            _log.LogDebug("Creating Selection parser");

            return OozeParserCreator.SelectionParser(_fieldDelimiter)
                       .Parse(fields);
        }
    }
}
