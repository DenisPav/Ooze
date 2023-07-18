﻿using Ooze.Typed.Expressions;
using System.Linq.Expressions;

namespace Ooze.Typed.Filters;

/// <inheritdoc />
internal class FilterBuilder<TEntity, TFilter> : IFilterBuilder<TEntity, TFilter>
{
    private readonly IList<FilterDefinition<TEntity, TFilter>> _filterDefinitions = new List<FilterDefinition<TEntity, TFilter>>();

    public IFilterBuilder<TEntity, TFilter> Equal<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => filterFunc(filter) != null,
            FilterExpressionFactory = filter => BasicExpressions.Equal(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> NotEqual<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => filterFunc(filter) != null,
            FilterExpressionFactory = filter => BasicExpressions.NotEqual(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> GreaterThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => filterFunc(filter) != null,
            FilterExpressionFactory = filter => BasicExpressions.GreaterThan(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> LessThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => filterFunc(filter) != null,
            FilterExpressionFactory = filter => BasicExpressions.LessThan(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> In<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter =>
            {
                var value = filterFunc(filter);
                return value != null && value.Any();
            },
            FilterExpressionFactory = filter => BasicExpressions.In(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> NotIn<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter =>
            {
                var value = filterFunc(filter);
                return value != null && value.Any();
            },
            FilterExpressionFactory = filter => BasicExpressions.In(dataExpression, filterFunc(filter), true)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> Range<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter =>
            {
                var value = filterFunc(filter);
                return value != null && value.From != null && value.To != null;
            },
            FilterExpressionFactory = filter => BasicExpressions.Range(dataExpression, filterFunc(filter))
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> OutOfRange<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter =>
            {
                var value = filterFunc(filter);
                return value != null && value.From != null && value.To != null;
            },
            FilterExpressionFactory = filter => BasicExpressions.Range(dataExpression, filterFunc(filter), true)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> StartsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => string.IsNullOrEmpty(filterFunc(filter)) == false,
            FilterExpressionFactory = filter
                => BasicExpressions.StringOperation(dataExpression, filterFunc(filter), CommonMethods.StringStartsWith)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> DoesntStartWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => string.IsNullOrEmpty(filterFunc(filter)) == false,
            FilterExpressionFactory = filter
                => BasicExpressions.StringOperation(dataExpression, filterFunc(filter), CommonMethods.StringStartsWith, true)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> EndsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => string.IsNullOrEmpty(filterFunc(filter)) == false,
            FilterExpressionFactory = filter
                => BasicExpressions.StringOperation(dataExpression, filterFunc(filter), CommonMethods.StringEndsWith)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> DoesntEndWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string?> filterFunc)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => string.IsNullOrEmpty(filterFunc(filter)) == false,
            FilterExpressionFactory = filter
                => BasicExpressions.StringOperation(dataExpression, filterFunc(filter), CommonMethods.StringEndsWith, true)
        });

        return this;
    }

    public IFilterBuilder<TEntity, TFilter> Add(
        Func<TFilter, bool> shouldRun,
        Func<TFilter, Expression<Func<TEntity, bool>>> filterExpressionFactory)
    {
        _filterDefinitions.Add(new FilterDefinition<TEntity, TFilter>
        {
            ShouldRun = shouldRun,
            FilterExpressionFactory = filterExpressionFactory
        });

        return this;
    }

    public IEnumerable<IFilterDefinition<TEntity, TFilter>> Build() => _filterDefinitions;
}
