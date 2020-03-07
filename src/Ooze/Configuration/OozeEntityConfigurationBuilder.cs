using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    public class OozeEntityConfigurationBuilder<TEntity>
        : IOozeEntityConfigurationBuilderInternal, IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        readonly ParameterExpression Parameter = Parameter(typeof(TEntity), nameof(TEntity));
        readonly IList<ExpressionDefinition> _sorters = new List<ExpressionDefinition>();
        readonly IList<ExpressionDefinition> _filters = new List<ExpressionDefinition>();

        private OozeEntityConfigurationBuilder()
        { }

        public static OozeEntityConfigurationBuilder<TEntity> Create() => new OozeEntityConfigurationBuilder<TEntity>();

        public IOozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            string sorterName,
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            _sorters.Add(new ExpressionDefinition
            {
                Name = sorterName,
                Expression = sortExpression
            });

            return this;
        }

        public IOozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            string memberName = GetExpressionName(sortExpression, "Sorter definition not correct");
            return Sort(memberName, sortExpression);
        }

        public IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(
            string filterName,
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            _filters.Add(new ExpressionDefinition
            {
                Name = filterName,
                Expression = sortExpression
            });

            return this;
        }

        public IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            string memberName = GetExpressionName(sortExpression, "Filter definition not correct");
            return Filter(memberName, sortExpression);
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

        public OozeEntityConfiguration Build() => new OozeEntityConfiguration
        {
            Type = typeof(TEntity),
            Param = Parameter,
            Sorters = CreateSorters(),
            Filters = CreateFilters()
        };

        IEnumerable<ParsedExpressionDefinition> CreateSorters() => CreateFor(_sorters, prop => Lambda(MakeMemberAccess(Parameter, prop), Parameter)).ToList();

        IEnumerable<ParsedExpressionDefinition> CreateFilters() => CreateFor(_filters, prop => MakeMemberAccess(Parameter, prop)).ToList();

        IEnumerable<ParsedExpressionDefinition> CreateFor(IEnumerable<ExpressionDefinition> expressionDefinitions, Func<PropertyInfo, Expression> exprSelector)
        {
            foreach (var definition in expressionDefinitions)
            {
                var lambdaSorter = definition.Expression as LambdaExpression;

                var memberName = (lambdaSorter.Body as MemberExpression).Member.Name;
                var prop = typeof(TEntity).GetProperty(memberName);
                var propType = prop.PropertyType;
                var expr = exprSelector(prop);

                yield return new ParsedExpressionDefinition
                {
                    Name = definition.Name,
                    Expression = expr,
                    Type = propType
                };
            }
        }
    }
}
