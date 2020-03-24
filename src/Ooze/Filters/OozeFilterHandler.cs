using Ooze.Configuration;
using Ooze.Parsers;
using Superpower;
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
            var filterProviders = _customProviderProvider.FiltersFor<TEntity>();
            var filterParser = CreateParser(filterProviders);
            var parsedFilters = GetParsedFilters(filters, filterParser);

            IQueryable<TEntity> Accumulator(IQueryable<TEntity> accumulator, FilterParserResult filter) 
                => OozeQueryableCreator.ForFilter(accumulator, filter, filterProviders);

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

            return parsedFilters;
        }

        TextParser<FilterParserResult> CreateParser(IEnumerable<IOozeProvider> customProviders)
        {
            var filterNames = customProviders.Select(provider => provider.Name);
            return OozeParserCreator.FilterParser(filterNames, _config.OperationsMap.Keys);
        }
    }
}
