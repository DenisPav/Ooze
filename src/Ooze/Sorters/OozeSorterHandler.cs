using Microsoft.Extensions.Logging;
using Ooze.Parsers;
using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Sorters
{
    internal class OozeSorterHandler : IOozeSorterHandler
    {
        const char _negativeOrderChar = '-';
        const char _sorterSeparator = ',';

        readonly IOozeProviderLocator _providerLocator;
        readonly ILogger<OozeSorterHandler> _log;

        public OozeSorterHandler(
            IOozeProviderLocator providerLocator,
            ILogger<OozeSorterHandler> log)
        {
            _providerLocator = providerLocator;
            _log = log;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string sorters)
            where TEntity : class
        {
            _log.LogDebug("Running sorter IQueryable changes");

            var sorterProviders = _providerLocator.SortersFor<TEntity>();
            var parsedSorters = GetParsedSorters(sorters);

            foreach (var parsedSorter in parsedSorters)            
            {
                var sorter = sorterProviders.SingleOrDefault(sorter => string.Equals(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase));

                if (sorter is { })
                {
                    query = (query is IOrderedQueryable<TEntity> orderQuery)
                        ? sorter.ThenApplySorter(orderQuery, parsedSorter.Ascending)
                        : sorter.ApplySorter(query, parsedSorter.Ascending);
                }
            }

            _log.LogDebug("Final sorter expression: {expression}", query.Expression.ToString());
            return query;
        }

        IEnumerable<SorterParserResult> GetParsedSorters(
            string sorters)
        {
            _log.LogDebug("Creating Sorter parser");

            var parser = OozeParserCreator.SorterParser(_negativeOrderChar);

            return sorters.Split(_sorterSeparator)
                .Select(sorter => sorter.Trim())
                .Select(parser.TryParse)
                .Where(result => result.HasValue)
                .Select(result => result.Value)
                .ToList();
        }
    }
}
