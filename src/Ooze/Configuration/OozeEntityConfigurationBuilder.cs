using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    public class OozeEntityConfigurationBuilder<TEntity>
        : IOozeEntityConfigurationBuilderInternal, IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        readonly ParameterExpression Parameter = Parameter(typeof(TEntity), nameof(TEntity));
        readonly IList<SorterExpression> _sorters = new List<SorterExpression>();

        private OozeEntityConfigurationBuilder()
        { }

        public static OozeEntityConfigurationBuilder<TEntity> Create()
        {
            return new OozeEntityConfigurationBuilder<TEntity>();
        }

        public OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            string sorterName,
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            _sorters.Add(new SorterExpression
            {
                Name = sorterName,
                Sorter = sortExpression
            });

            return this;
        }

        public OozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            string memberName = GetExpressionName(sortExpression, "Sorter definition not correct");
            return Sort(memberName, sortExpression);
        }

        private static string GetExpressionName<TTarget>(
            Expression<Func<TEntity, TTarget>> expression,
            string error)
        {
            if (!(expression.Body is MemberExpression))
            {
                throw new Exception(error);
            }

            var memberName = (expression.Body as MemberExpression).Member.Name;
            return memberName;
        }

        public OozeEntityConfiguration Build()
        {
            return new OozeEntityConfiguration
            {
                Type = typeof(TEntity),
                Sorters = new Expressions
                {
                    Param = Parameter,
                    LambdaExpressions = CreateSorters().ToList()
                }
            };
        }

        IEnumerable<(string, LambdaExpression, Type)> CreateSorters()
        {
            foreach (var sorter in _sorters)
            {
                var lambdaSorter = sorter.Sorter as LambdaExpression;

                if (!(lambdaSorter.Body is MemberExpression))
                {
                    throw new Exception("Sorter definition not correct");
                }

                var memberName = (lambdaSorter.Body as MemberExpression).Member.Name;
                var prop = typeof(TEntity).GetProperty(memberName);
                var propType = prop.PropertyType;

                var memberAccess = MakeMemberAccess(Parameter, prop);
                var lambda = Lambda(memberAccess, Parameter);

                yield return (sorter.Name, lambda, propType);
            }
        }
    }
}
