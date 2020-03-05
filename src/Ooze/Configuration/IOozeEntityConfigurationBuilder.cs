using System;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    public interface IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(string sorterName, Expression<Func<TEntity, TTarget>> sortExpression);
        OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(Expression<Func<TEntity, TTarget>> sortExpression);
    }
}
