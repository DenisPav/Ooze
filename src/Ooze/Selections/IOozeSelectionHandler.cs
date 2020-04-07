using System;
using System.Linq;
using static System.Linq.Expressions.Expression;

namespace Ooze.Selections
{
    internal interface IOozeSelectionHandler
    {
        IQueryable<TEntity> Handle<TEntity>(IQueryable<TEntity> query, string fields)
            where TEntity : class;
    }

    internal class OozeSelectionHandler : IOozeSelectionHandler
    {
        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string fields)
            where TEntity : class
        {
            var paramExpr = Parameter(
                   typeof(TEntity),
                   typeof(TEntity).Name
               );

            var bindExpressions = typeof(TEntity).GetProperties()
                .Where(prop => fields.Contains(prop.Name, StringComparison.InvariantCultureIgnoreCase))
                .Select(prop => Bind(prop, MakeMemberAccess(paramExpr, prop)));

            var lambda = Lambda<Func<TEntity, TEntity>>(
                MemberInit(
                    New(
                        typeof(TEntity).GetConstructors().First()
                    ),
                    bindExpressions
                ),
                paramExpr
            );

            return query.Select(lambda);
        }
    }
}
