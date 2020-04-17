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

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string fields)
            where TEntity : class
        {
            var results = GetParsedSelections(fields);
            var paramExpr = Parameter(typeof(TEntity), typeof(TEntity).Name);
            var assignments = OozeExpressionCreator.CreateAssignments(paramExpr, typeof(TEntity), results);

            if (OozeExpressionCreator.LambdaExpr(typeof(TEntity), paramExpr, assignments) is Expression<Func<TEntity, TEntity>> lambda)
            {
                query = query.Select(lambda);
            }

            return query;
        }

        static IEnumerable<FieldDefinition> GetParsedSelections(string fields) 
            => OozeParserCreator.SelectionParser(_fieldDelimiter)
            .Parse(fields);       
    }
}
