using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Ooze.Expressions
{
    public interface IExpressionResolver
    {
        IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, object model);
    }

    public class ExpressionResolver : IExpressionResolver
    {
        private readonly OozeConfiguration _config;

        public ExpressionResolver(
            IOptions<OozeConfiguration> config)
        {
            _config = config.Value;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, object model)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations.FirstOrDefault(config => config.Type.Equals(entity));

            foreach (var sorter in configuration.Sorters.LambdaExpressions)
            {
                //this needs to be updated this is just here to try the whole flow
                var callExpr = Call(typeof(Queryable), "OrderBy", new Type[] { entity, sorter.Item2 },
                                          query.Expression, Quote(sorter.Item1));

                return query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }
    }
}
