using Ooze.Configuration;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze
{
    public class OozeResolver : IOozeResolver
    {
        const string OrderBy = nameof(OrderBy);
        const string ThenBy = nameof(ThenBy);
        const string ThenByDescending = nameof(ThenByDescending);
        const string OrderByDescending = nameof(OrderByDescending);
        const string Where = nameof(Where);

        static readonly Type QueryableType = typeof(Queryable);

        private readonly OozeConfiguration _config;

        public OozeResolver(
            OozeConfiguration config)
        {
            _config = config;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
        {
            query = HandleSorters(query, model.Sorters);
            query = HandleFilters(query, model.Filters);

            return query;
        }

        IQueryable<TEntity> HandleFilters<TEntity>(IQueryable<TEntity> query, string filters)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations.FirstOrDefault(config => config.Type.Equals(entity));

            var propertyParsers = configuration.Filters.LambdaExpressions.Select(def => Span.EqualToIgnoreCase(def.Item1)).ToList();
            var propertyParser = propertyParsers.Aggregate((TextParser<TextSpan>)null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator.Or(singlePropertyParser);
            }).OptionalOrDefault(new TextSpan(string.Empty));

            var operationParser = Character.EqualTo('=').Optional();
            var valueParser = Span.WithAll(_ => true).OptionalOrDefault(new TextSpan(string.Empty));
            var filterParser = (from property in propertyParser
                                from operation in operationParser
                                from value in valueParser
                                select (property, operation, value));

            var splittedFilters = filters.Split(',').ToList();
            var parsedFilters = splittedFilters.Select(filterParser.Parse).ToList();

            var appliedFilters = parsedFilters.Join(
                configuration.Filters.LambdaExpressions,
                x => x.property.ToString(),
                x => x.Item1,
                (x, y) => (parsed: x, def: y),
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            foreach (var (parsed, def) in appliedFilters)
            {
                var expr = query.Expression;
                var typings = new[] { entity };

                var value = System.Convert.ChangeType(parsed.value.ToString(), def.Item3);
                var constValueExpr = Constant(value);
                var equalExpr = Equal(def.Item2, constValueExpr);
                var lambda = Lambda(equalExpr, configuration.Filters.Param);

                var quoteExpr = Quote(lambda);

                var callExpr = Call(QueryableType, Where, typings, expr, quoteExpr);

                query = query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }

        IQueryable<TEntity> HandleSorters<TEntity>(IQueryable<TEntity> query, string sorters)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations.FirstOrDefault(config => config.Type.Equals(entity));

            if (string.IsNullOrEmpty(sorters) || string.IsNullOrWhiteSpace(sorters))
                return query;

            var modelSorters = sorters.Split(',').Select(sorter => sorter.Trim())
                .Select(sorter => new
                {
                    Ascending = !sorter.StartsWith('-') ? true : false,
                    Sorter = sorter.StartsWith('-') ? new string(sorter.Skip(1).ToArray()) : sorter
                })
                .ToList();

            var appliedSorters = modelSorters.Join(
                configuration.Sorters.LambdaExpressions,
                x => x.Sorter,
                x => x.Item1,
                (x, y) => (y.Item1, y.Item2, y.Item3, x.Ascending),
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            for (int i = 0; i < appliedSorters.Count(); i++)
            {
                var expr = query.Expression;
                var sorter = appliedSorters[i];
                MethodCallExpression callExpr;
                var quoteExpr = Quote(sorter.Item2);
                var typings = new Type[] { entity, sorter.Item3 };

                if (i == 0)
                {
                    var method = sorter.Ascending ? OrderBy : OrderByDescending;
                    callExpr = Call(QueryableType, method, typings, expr, quoteExpr);
                }
                else
                {
                    var method = sorter.Ascending ? ThenBy : ThenByDescending;
                    callExpr = Call(QueryableType, method, typings, expr, quoteExpr);
                }

                query = query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }
    }
}
