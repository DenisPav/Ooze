using Microsoft.Extensions.Logging;
using Ooze.Configuration;
using System.Linq;

namespace Ooze.Paging
{
    internal class OozePagingHandler : IOozePagingHandler
    {
        readonly OozeConfiguration _config;
        readonly ILogger<OozePagingHandler> _log;

        public OozePagingHandler(
            OozeConfiguration config,
            ILogger<OozePagingHandler> log)
        {
            _config = config;
            _log = log;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            int? page,
            int? pageSize)
        {
            _log.LogDebug("Running paging IQueryable changes");

            var actualPage = page.GetValueOrDefault(0);
            var actualPageSize = pageSize.GetValueOrDefault(_config.DefaultPageSize);

            var toSkip = actualPage * actualPageSize;
            var toTake = actualPageSize;

            query = query.Skip(toSkip)
                .Take(toTake);

            _log.LogDebug("Final paging expression: {expression}", query.Expression.ToString());
            return query;
        }
    }
}
