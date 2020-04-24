﻿using Ooze.Configuration;
using System.Linq;

namespace Ooze.Paging
{
    internal interface IOozePagingHandler
    {
        IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            int? page,
            int? pageSize);
    }

    internal class OozePagingHandler : IOozePagingHandler
    {
        readonly OozeConfiguration _config;

        public OozePagingHandler(OozeConfiguration config) => _config = config;

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            int? page,
            int? pageSize)
        {
            var actualPage = page.GetValueOrDefault(0);
            var actualPageSize = pageSize.GetValueOrDefault(_config.DefaultPageSize);

            var toSkip = actualPage * actualPageSize;
            var toTake = actualPageSize + 1;

            return query.Skip(toSkip)
                .Take(toTake);
        }
    }
}
