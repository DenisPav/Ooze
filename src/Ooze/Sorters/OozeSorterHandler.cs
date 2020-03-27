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

        readonly IOozeProviderLocator _providerLocator;

        public OozeSorterHandler(IOozeProviderLocator providerLocator) => _providerLocator = providerLocator;

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string sorters)
            where TEntity : class
        {
            var sorterProviders = _providerLocator.SortersFor<TEntity>();
            var parsedSorters = GetParsedSorters(sorters).ToList();

            for (int i = 0; i < parsedSorters.Count(); i++)
            {
                //not ThenBy call
                var isFirst = i == 0;
                var parsedSorter = parsedSorters[i];
                var sorter = sorterProviders.SingleOrDefault(sorter => string.Equals(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase));

                query = isFirst
                    ? sorter.ApplySorter(query, parsedSorter.Ascending)
                    : sorter.ThenApplySorter(query as IOrderedQueryable<TEntity>, parsedSorter.Ascending);
            }

            return query;
        }

        IEnumerable<SorterParserResult> GetParsedSorters(
            string sorters)
        {
            var parser = OozeParserCreator.SorterParser(_negativeOrderChar);

            return sorters.Split(',')
                .Select(sorter => sorter.Trim())
                .Select(parser.TryParse)
                .Where(result => result.HasValue)
                .Select(result => result.Value);
        }
    }
}
