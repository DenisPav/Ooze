using Ooze.Configuration;
using Ooze.Parsers;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Filters
{
    internal class OozeFilterHandler : IOozeFilterHandler
    {
        readonly IOozeProviderProvider _providerProvider;
        readonly OozeConfiguration _config;

        public OozeFilterHandler(
            IOozeProviderProvider providerProvider,
            OozeConfiguration config)
        {
            _providerProvider = providerProvider;
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string filters)
            where TEntity : class
        {
            var filterProviders = _providerProvider.FiltersFor<TEntity>();
            var filterParser = CreateParser(filterProviders);
            var parsedFilters = GetParsedFilters(filters, filterParser);

            IQueryable<TEntity> Accumulator(IQueryable<TEntity> accumulator, FilterParserResult filter)
            {
                var filterProvider = filterProviders.SingleOrDefault(configFilter => string.Equals(configFilter.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase));
                return filterProvider.ApplyFilter(query, filter);
            }

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
