using Ooze.Filters;
using System.Collections.Generic;

namespace Ooze
{
    internal interface IOozeCustomProviderProvider
    {
        IEnumerable<IOozeFilterProvider<TEntity>> FiltersFor<TEntity>()
            where TEntity : class;
    }
}
