using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Ooze.Expressions
{
    public interface IExpressionResolver
    {
        IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model);
    }

    public class ExpressionResolver : IExpressionResolver
    {
        const string OrderBy = nameof(OrderBy);
        const string ThenBy = nameof(ThenBy);
        static readonly Type QueryableType = typeof(Queryable);

        private readonly OozeConfiguration _config;

        public ExpressionResolver(
            IOptions<OozeConfiguration> config)
        {
            _config = config.Value;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query, OozeModel model)
        {
            query = HandleSorters(query, model.Sorters);

            return query;
        }

        IQueryable<TEntity> HandleSorters<TEntity>(IQueryable<TEntity> query, string sorters)
        {
            var entity = typeof(TEntity);
            var configuration = _config.EntityConfigurations.FirstOrDefault(config => config.Type.Equals(entity));

            if (string.IsNullOrEmpty(sorters) || string.IsNullOrWhiteSpace(sorters))
                return query;

            var modelSorters = sorters.Split(',').Select(sorter => sorter.Trim())
                .ToList();

            var appliedSorters = modelSorters.Join(
                configuration.Sorters.LambdaExpressions,
                x => x,
                x => x.Item1,
                (x, y) => y,
                StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            for (int i = 0; i < appliedSorters.Count(); i++)
            {
                var expr = query.Expression;
                var sorter = appliedSorters[i];
                MethodCallExpression callExpr;
                var quoteExpr = Quote(sorter.Item2);
                var typings = new Type[] { entity, sorter.Item3 };

                if (i == 0)
                {
                    callExpr = Call(QueryableType, OrderBy, typings, expr, quoteExpr);
                }
                else
                {
                    callExpr = Call(QueryableType, ThenBy, typings, expr, quoteExpr);
                }

                query = query.Provider
                    .CreateQuery<TEntity>(callExpr);
            }

            return query;
        }
    }

    public class OozeModel
    {
        public string Sorters { get; set; }
    }
}
