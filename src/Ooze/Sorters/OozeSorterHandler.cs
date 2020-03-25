using Superpower;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Sorters
{
    internal class OozeSorterHandler : IOozeSorterHandler
    {
        const char _negativeOrderChar = '-';

        readonly IOozeProviderProvider _providerProvider;

        public OozeSorterHandler(IOozeProviderProvider providerProvider) => _providerProvider = providerProvider;

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string sorters)
            where TEntity : class
        {
            var sorterProviders = _providerProvider.SortersFor<TEntity>();
            var parsedSorters = GetParsedSorters(sorters).ToList();

            for (int i = 0; i < parsedSorters.Count(); i++)
            {
                //not ThenBy call
                var isFirst = i == 0;
                var parsedSorter = parsedSorters[i];
                var sorter = sorterProviders.SingleOrDefault(sorter => string.Compare(sorter.Name, parsedSorter.Sorter, StringComparison.InvariantCultureIgnoreCase) == 0);

                query = isFirst
                    ? sorter.ApplySorter(query, parsedSorter.Ascending)
                    : sorter.ThenApplySorter(query as IOrderedQueryable<TEntity>, parsedSorter.Ascending);
            }

            return query;
        }

        IEnumerable<SorterParserResult> GetParsedSorters(
            string sorters)
        {
            return sorters.Split(',')
                .Select(sorter => sorter.Trim())
                .Select(sorter => new SorterParserResult
                {
                    Ascending = !sorter.StartsWith(_negativeOrderChar) ? true : false,
                    Sorter = sorter.StartsWith(_negativeOrderChar) ? new string(sorter.Skip(1).ToArray()) : sorter
                });
        }
    }
}
