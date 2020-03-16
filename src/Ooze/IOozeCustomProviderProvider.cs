using Ooze.Filters;
using Ooze.Sorters;
using System.Collections.Generic;

namespace Ooze
{
    internal interface IOozeCustomProviderProvider
    {
        IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>()
            where TEntity : class;
        IEnumerable<IOozeSorterProvider<TEntity>> SortersFor<TEntity>()
            where TEntity : class;
    }
}
