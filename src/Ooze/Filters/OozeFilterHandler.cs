using Ooze.Configuration;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Filters
{
    internal class OozeFilterHandler : IOozeFilterHandler
    {
        const string Where = nameof(Where);
        static readonly Type _queryableType = typeof(Queryable);
        static readonly Type _stringType = typeof(string);

        readonly IOozeCustomProviderProvider _customProviderProvider;
        readonly OozeConfiguration _config;

        public OozeFilterHandler(
            IOozeCustomProviderProvider customProviderProvider,
            OozeConfiguration config)
        {
            _customProviderProvider = customProviderProvider;
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string filters)
            where TEntity : class
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations[entity];
            var customProviders = _customProviderProvider.FiltersFor<TEntity>();

            var filterParser = CreateParser(configuration, customProviders);
            var parsedFilters = GetParsedFilters(filters, filterParser);
            var appliedConfigurationFilters = GetAppliedFilters(parsedFilters, configuration);

            foreach (var filter in parsedFilters)
            {
                var expr = query.Expression;

                var configFilter = appliedConfigurationFilters.SingleOrDefault(configFilter => string.Equals(configFilter.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase));
                if (configFilter != null)
                {
                    var callExpr = CreateFilterExpression(entity, configuration, filter, configFilter, expr);
                    query = query.Provider
                        .CreateQuery<TEntity>(callExpr);
                }
                else
                {
                    var providerQuery = customProviders.SingleOrDefault(provider => string.Equals(provider.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase))
                        ?.ApplyFilter(query, filter);

                    if (providerQuery != null)
                        query = providerQuery;
                }
            }

            return query;
        }

        IEnumerable<FilterParserResult> GetParsedFilters(
            string filters,
            TextParser<FilterParserResult> filterParser)
        {
            var splittedFilters = filters.Split(',');
            var parsedFilters = splittedFilters.Select(filterParser.TryParse)
                .Where(result => result.HasValue)
                .Select(result => result.Value);

            return parsedFilters.ToList();
        }

        MethodCallExpression CreateFilterExpression(
            Type entity,
            OozeEntityConfiguration configuration,
            FilterParserResult parsedFilter,
            ParsedExpressionDefinition def,
            Expression expr)
        {
            var typings = new[] { entity };

            var typeConverter = TypeDescriptor.GetConverter(def.Type);
            var value = typeConverter.CanConvertFrom(_stringType)
                ? typeConverter.ConvertFrom(parsedFilter.Value)
                : System.Convert.ChangeType(parsedFilter.Value, def.Type);

            var constValueExpr = Constant(value);
            var operationExpr = _config.OperationsMap[parsedFilter.Operation](def.Expression, constValueExpr);
            var lambda = Lambda(operationExpr, configuration.Param);
            var quoteExpr = Quote(lambda);

            var callExpr = Call(_queryableType, Where, typings, expr, quoteExpr);
            return callExpr;
        }

        IEnumerable<ParsedExpressionDefinition> GetAppliedFilters(
            IEnumerable<FilterParserResult> parsedFilters,
            OozeEntityConfiguration configuration)
        {
            var distinctParsedFilters = parsedFilters.Select(filter => filter.Property)
                .Distinct();

            var appliedFilters = distinctParsedFilters.Join(
                configuration.Filters,
                parsedFilter => parsedFilter,
                configuredFilter => configuredFilter.Name,
                (parsedFilter, configuredFilter) => configuredFilter,
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            return appliedFilters;
        }

        TextParser<FilterParserResult> CreateParser(OozeEntityConfiguration configuration, IEnumerable<IOozeProvider> customProviders)
        {
            var whiteSpaceParser = Character.WhiteSpace.Many();
            var propertyParsers = configuration.Filters.Select(def => Span.EqualToIgnoreCase(def.Name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();
            var customFilterParsers = customProviders.Select(def => Span.EqualToIgnoreCase(def.Name).Between(whiteSpaceParser, whiteSpaceParser)).ToList();

            var propertyParser = propertyParsers.Concat(customFilterParsers)
                .Aggregate<TextParser<TextSpan>, TextParser<TextSpan>>(null, (accumulator, singlePropertyParser) =>
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
                                select new FilterParserResult(property.ToString(), operation.ToString(), value.ToString()));

            return filterParser;
        }
    }
}
