using System;
using System.Linq.Expressions;

namespace Ooze.Configuration
{
    public interface IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        IOozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(string sorterName, Expression<Func<TEntity, TTarget>> sortExpression);
        IOozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(Expression<Func<TEntity, TTarget>> sortExpression);
        IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(string filterName, Expression<Func<TEntity, TTarget>> filterExpression);
        IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(Expression<Func<TEntity, TTarget>> filterExpression);
    }
}
