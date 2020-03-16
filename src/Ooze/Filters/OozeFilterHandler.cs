using Ooze.Configuration;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Filters
{
    internal class OozeFilterHandler : IOozeFilterHandler
    {
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
            var configuration = _config.EntityConfigurations[typeof(TEntity)];
            var customProviders = _customProviderProvider.FiltersFor<TEntity>();

            var filterParser = CreateParser(configuration, customProviders);
            var parsedFilters = GetParsedFilters(filters, filterParser);

            IQueryable<TEntity> Accumulator(IQueryable<TEntity> accumulator, FilterParserResult filter) 
                => OozeQueryableCreator.ForFilter(accumulator, configuration, filter, customProviders, _config.OperationsMap);

            return parsedFilters.Aggregate(query, Accumulator);
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
