﻿using Microsoft.Extensions.Logging;
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
        readonly IOozeProviderLocator _providerLocator;
        readonly OozeConfiguration _config;
        readonly ILogger<OozeFilterHandler> _log;

        public OozeFilterHandler(
            IOozeProviderLocator providerLocator,
            OozeConfiguration config,
            ILogger<OozeFilterHandler> log)
        {
            _providerLocator = providerLocator;
            _config = config;
            _log = log;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string filters)
            where TEntity : class
        {
            _log.LogDebug("Running filter IQueryable changes");

            var filterProviders = _providerLocator.FiltersFor<TEntity>();
            var filterParser = CreateParser(filterProviders);
            var parsedFilters = GetParsedFilters(filters, filterParser);

            IQueryable<TEntity> Accumulator(IQueryable<TEntity> accumulator, FilterParserResult filter)
            {
                var filterProvider = filterProviders.SingleOrDefault(configFilter => string.Equals(configFilter.Name, filter.Property, StringComparison.InvariantCultureIgnoreCase));
                return filterProvider.ApplyFilter(accumulator, filter);
            }

            query = parsedFilters.Aggregate(query, Accumulator);

            _log.LogDebug("Final filter expression: {expression}", query.Expression.ToString());
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

            return parsedFilters;
        }

        TextParser<FilterParserResult> CreateParser(IEnumerable<IOozeProvider> customProviders)
        {
            _log.LogDebug("Creating Filter parser");

            var filterNames = customProviders.Select(provider => provider.Name);
            return OozeParserCreator.FilterParser(filterNames, _config.OperationsMap.Keys);
        }
    }
}
