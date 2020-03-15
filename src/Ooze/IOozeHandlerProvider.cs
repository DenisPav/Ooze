using Ooze.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ooze
{
    public interface IOozeProvider
    {
        string Name { get; }
    }

    public interface IOozeCustomProviderProvider
    {
        IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>()
            where TEntity : class;
    }

    class OozeCustomProviderProvider : IOozeCustomProviderProvider
    {
        readonly IEnumerable<IOozeProvider> _customProviders;

        public OozeCustomProviderProvider(
            IEnumerable<IOozeProvider> customProviders)
        {
            _customProviders = customProviders;
        }

        public IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>() where TEntity : class
        {
            return _customProviders.Cast<IOozeFilterProvider<TEntity>>()
                .Where(provider => provider != null)
                .ToList();

        }
    }
}
