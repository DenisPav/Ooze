using Ooze.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Ooze
{
    internal class OozeCustomProviderProvider : IOozeCustomProviderProvider
    {
        readonly IEnumerable<IOozeProvider> _customProviders;

        public OozeCustomProviderProvider(
            IEnumerable<IOozeProvider> customProviders)
        {
            _customProviders = customProviders;
        }

        public IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>() where TEntity : class
        {
            return _customProviders.Select(provider => provider as IOozeFilterProvider<TEntity>)
                .Where(provider => provider != null)
                .ToList();
        }
    }
}
