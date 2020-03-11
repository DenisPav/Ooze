﻿using Ooze.Configuration;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Filters
{
    public class OozeFilterHandler : IOozeFilterHandler
    {
        const string Where = nameof(Where);
        static readonly Type _queryableType = typeof(Queryable);

        readonly OozeConfiguration _config;

        public OozeFilterHandler(
            OozeConfiguration config)
        {
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string filters)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations[entity];

            var filterParser = CreateParser(configuration);
            var appliedFilters = GetAppliedFilters(filters, configuration, filterParser);

            foreach (var (parsed, def) in appliedFilters)
            {
                var expr = query.Expression;
                var callExpr = CreateFilterExpression(entity, configuration, parsed, def, expr);

                query = query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }

        MethodCallExpression CreateFilterExpression(
            Type entity,
            OozeEntityConfiguration configuration,
            (TextSpan property, TextSpan operation, TextSpan value) parsed,
            ParsedExpressionDefinition def,
            Expression expr)
        {
            var typings = new[] { entity };
            var value = System.Convert.ChangeType(parsed.value.ToString(), def.Type);

            var constValueExpr = Constant(value);
            var operationExpr = _config.OperationsMap[parsed.operation.ToString()](def.Expression, constValueExpr);
            var lambda = Lambda(operationExpr, configuration.Param);
            var quoteExpr = Quote(lambda);

            var callExpr = Call(_queryableType, Where, typings, expr, quoteExpr);
            return callExpr;
        }

        IEnumerable<((TextSpan property, TextSpan operation, TextSpan value) parsed, ParsedExpressionDefinition def)> GetAppliedFilters(
            string filters,
            OozeEntityConfiguration configuration,
            TextParser<(TextSpan property, TextSpan operation, TextSpan value)> filterParser)
        {
            var splittedFilters = filters.Split(',');
            var parsedFilters = splittedFilters.Select(filterParser.TryParse)
                .Where(result => result.HasValue)
                .Select(result => result.Value);

            var appliedFilters = parsedFilters.Join(
                configuration.Filters,
                x => x.property.ToString(),
                x => x.Name,
                (x, y) => (parsed: x, def: y),
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            return appliedFilters;
        }

        TextParser<(TextSpan property, TextSpan operation, TextSpan value)> CreateParser(OozeEntityConfiguration configuration)
        {
            var whiteSpaceParser = Character.WhiteSpace.Many();
            var propertyParsers = configuration.Filters.Select(def => Span.EqualToIgnoreCase(def.Name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();
            var propertyParser = propertyParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator.Or(singlePropertyParser);
            });

            var operationParsers = _config.OperationsMap.Keys.Select(Span.EqualToIgnoreCase).ToList();
            var operationParser = operationParsers.Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
            {
                if (accumulator == null)
                    return singlePropertyParser;

                return accumulator
                    .Try()
                    .Or(singlePropertyParser);
            });

            var valueParser = Span.WithAll(_ => true).OptionalOrDefault(new TextSpan(string.Empty));
            var filterParser = (from property in propertyParser
                                from operation in operationParser
                                from value in valueParser
                                select (property, operation, value));

            return filterParser;
        }
    }
}
