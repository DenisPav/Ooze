using Microsoft.Extensions.DependencyInjection;
using Ooze.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Ooze.Configuration
{
    internal class OozeEntityConfigurationBuilder<TEntity>
        : IOozeEntityConfigurationBuilderInternal, IOozeEntityConfigurationBuilder<TEntity>
        where TEntity : class
    {
        const string _sorterError = "Sorter definition not correct";
        const string _filterError = "Filter definition not correct";

        readonly ParameterExpression _parameter = Parameter(typeof(TEntity), nameof(TEntity));
        readonly IList<ExpressionDefinition> _sorters = new List<ExpressionDefinition>();
        readonly IList<ExpressionDefinition> _filters = new List<ExpressionDefinition>();

        private OozeEntityConfigurationBuilder()
        { }

        public static OozeEntityConfigurationBuilder<TEntity> Create() => new OozeEntityConfigurationBuilder<TEntity>();

        public IOozeEntityConfigurationBuilder<TEntity> Sort<TTarget>(
            string sorterName,
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            ValidateExpression(sortExpression, _sorterError);
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
            string memberName = GetExpressionName(sortExpression, _sorterError);
            return Sort(memberName, sortExpression);
        }

        public IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(
            string filterName,
            Expression<Func<TEntity, TTarget>> filterExpression)
        {
            ValidateExpression(filterExpression, _filterError);
            _filters.Add(new ExpressionDefinition
            {
                Name = filterName,
                Expression = filterExpression
            });

            return this;
        }

        public IOozeEntityConfigurationBuilder<TEntity> Filter<TTarget>(
            Expression<Func<TEntity, TTarget>> sortExpression)
        {
            string memberName = GetExpressionName(sortExpression, _filterError);
            return Filter(memberName, sortExpression);
        }

        public (Type entityType, OozeEntityConfiguration configuration) Build() => (typeof(TEntity), new OozeEntityConfiguration
        {
            Param = _parameter,
            Sorters = CreateSorters(),
            Filters = CreateFilters()
        });

        static string GetExpressionName<TTarget>(
            Expression<Func<TEntity, TTarget>> expression,
            string error)
        {
            ValidateExpression(expression, error);

            var memberName = (expression.Body as MemberExpression).Member.Name;
            return memberName;
        }

        static void ValidateExpression<TTarget>(Expression<Func<TEntity, TTarget>> expression, string error)
        {
            if (!(expression.Body is MemberExpression))
            {
                throw new Exception(error);
            }
        }

        IEnumerable<ParsedExpressionDefinition> CreateSorters() => CreateFor(_sorters, prop => Lambda(MakeMemberAccess(_parameter, prop), _parameter), CreateSorterFactory).ToList();

        IEnumerable<ParsedExpressionDefinition> CreateFilters() => CreateFor(_filters, prop => MakeMemberAccess(_parameter, prop), CreateFilterFactory).ToList();

        IEnumerable<ParsedExpressionDefinition> CreateFor(
            IEnumerable<ExpressionDefinition> expressionDefinitions,
            Func<PropertyInfo, Expression> exprSelector,
            CreateProvider createProviderDelegate)
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
                    Type = propType,
                    ProviderFactory = createProviderDelegate(definition.Name, expr, propType)
                };
            }
        }

        delegate Func<IServiceProvider, IOozeProvider> CreateProvider(string name, Expression expression, Type propType);

        static Func<IServiceProvider, IOozeProvider> CreateFilterFactory(string name, Expression expression, Type propType) 
            => sp => new OozeFilterProvider<TEntity>(sp.GetRequiredService<OozeConfiguration>(), name);

        static Func<IServiceProvider, IOozeProvider> CreateSorterFactory(string name, Expression expression, Type propType)
            => sp => new OozeSorterProvider<TEntity>(name, expression, propType);
    }
}
