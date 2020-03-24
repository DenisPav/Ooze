using Ooze.Configuration;
using Superpower;
using System.Collections.Generic;
using System.Linq;

namespace Ooze.Sorters
{
    internal class OozeSorterHandler : IOozeSorterHandler
    {
        const char _negativeOrderChar = '-';

        readonly IOozeCustomProviderProvider _customProviderProvider;
        readonly OozeConfiguration _config;

        public OozeSorterHandler(
            IOozeCustomProviderProvider customProviderProvider,
            OozeConfiguration config)
        {
            _customProviderProvider = customProviderProvider;
            _config = config;
        }

        public IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string sorters)
            where TEntity : class
        {
            var customProviders = _customProviderProvider.SortersFor<TEntity>();
            var parsedSorters = GetParsedSorters(sorters).ToList();

            for (int i = 0; i < parsedSorters.Count(); i++)
            {
                //not ThenBy call
                var isFirst = i == 0;
                var parsedSorter = parsedSorters[i];

                query = OozeQueryableCreator.ForSorter(query, parsedSorter, customProviders, isFirst);
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
