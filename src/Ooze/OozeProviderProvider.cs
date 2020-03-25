using Ooze.Filters;
using Ooze.Sorters;
using System.Collections.Generic;
using System.Linq;

namespace Ooze
{
    internal class OozeProviderProvider : IOozeProviderProvider
    {
        readonly IEnumerable<IOozeProvider> _customProviders;

        public OozeProviderProvider(
            IEnumerable<IOozeProvider> customProviders)
        {
            _customProviders = customProviders;
        }

        public IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>()
            where TEntity : class
            => NonNullProvider<IOozeFilterProvider<TEntity>>();

        public IEnumerable<IOozeSorterProvider<TEntity>> SortersFor<TEntity>()
            where TEntity : class
            => NonNullProvider<IOozeSorterProvider<TEntity>>();

        IEnumerable<TProvider> NonNullProvider<TProvider>()
            where TProvider : class
        {
            return _customProviders.Select(provider => provider as TProvider)
                .Where(provider => provider != null)
                .ToList();
        }
    }
}
