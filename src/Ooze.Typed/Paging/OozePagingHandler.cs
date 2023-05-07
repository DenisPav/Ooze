using Microsoft.Extensions.Logging;

namespace Ooze.Typed.Paging;

internal class OozePagingHandler<TEntity> : IOozePagingHandler<TEntity>
{
    private readonly ILogger<OozePagingHandler<TEntity>> _log;

    public OozePagingHandler(ILogger<OozePagingHandler<TEntity>> log) => _log = log;

    public IQueryable<TEntity> Apply(
        IQueryable<TEntity> query,
        PagingOptions pagingOptions)
    {
        _log.LogDebug("Applying paging options! Page: [{page}], Page size: [{size}]", pagingOptions.Page,
            pagingOptions.Size);

        return query.Skip(pagingOptions.Page * pagingOptions.Size)
            .Take(pagingOptions.Size);
    }
}