using System.Linq.Expressions;
using Ooze.Typed.Expressions;

namespace Ooze.Typed.Filters.Async;

/// <inheritdoc />
internal class AsyncFilterBuilder<TEntity, TFilter> : IAsyncFilterBuilder<TEntity, TFilter>
{
    private readonly IList<AsyncFilterDefinition<TEntity, TFilter>> _filterDefinitions =
        new List<AsyncFilterDefinition<TEntity, TFilter>>();

    public IAsyncFilterBuilder<TEntity, TFilter> Equal<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => filterFunc(filter) != null;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filter => Task.FromResult(shouldRun(filter)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.Equal(
                    dataExpression,
                    filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> NotEqual<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => filterFunc(filter) != null;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter => Task.FromResult(BasicExpressions.NotEqual(
                dataExpression,
                filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> GreaterThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => filterFunc(filter) != null;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.GreaterThan(
                    dataExpression,
                    filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> LessThan<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, TProperty> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => filterFunc(filter) != null;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.LessThan(
                    dataExpression,
                    filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> In<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter =>
        {
            var value = filterFunc(filter);
            return value != null && value.Any();
        };
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter => Task.FromResult(BasicExpressions.In(
                dataExpression,
                filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> NotIn<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, IEnumerable<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter =>
        {
            var value = filterFunc(filter);
            return value != null && value.Any();
        };
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.In(
                    dataExpression,
                    filterFunc(filter),
                    true))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> Range<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter =>
        {
            var value = filterFunc(filter);
            return value != null && value.From != null && value.To != null;
        };
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.Range(
                    dataExpression,
                    filterFunc(filter)))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> OutOfRange<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        Func<TFilter, RangeFilter<TProperty>?> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter =>
        {
            var value = filterFunc(filter);
            return value != null && value.From != null && value.To != null;
        };
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter =>
                Task.FromResult(BasicExpressions.Range(
                    dataExpression,
                    filterFunc(filter),
                    true))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> StartsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => string.IsNullOrEmpty(filterFunc(filter)) == false;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter
                => Task.FromResult(BasicExpressions.StringOperation(
                    dataExpression,
                    filterFunc(filter),
                    CommonMethods.StringStartsWith))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> DoesntStartWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => string.IsNullOrEmpty(filterFunc(filter)) == false;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter => Task.FromResult(BasicExpressions.StringOperation(
                dataExpression,
                filterFunc(filter),
                CommonMethods.StringStartsWith,
                true))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> EndsWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => string.IsNullOrEmpty(filterFunc(filter)) == false;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter
                => Task.FromResult(BasicExpressions.StringOperation(
                    dataExpression,
                    filterFunc(filter),
                    CommonMethods.StringEndsWith))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> DoesntEndWith(
        Expression<Func<TEntity, string>> dataExpression,
        Func<TFilter, string> filterFunc,
        Func<TFilter, bool>? shouldRun = null)
    {
        shouldRun ??= filter => string.IsNullOrEmpty(filterFunc(filter)) == false;
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filter
                => Task.FromResult(BasicExpressions.StringOperation(
                    dataExpression,
                    filterFunc(filter),
                    CommonMethods.StringEndsWith,
                    true))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> Add(
        Func<TFilter, bool> shouldRun,
        Func<TFilter, Expression<Func<TEntity, bool>>> filterExpressionFactory)
    {
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = filters => Task.FromResult(shouldRun(filters)),
            FilterExpressionFactory = filters => Task.FromResult(filterExpressionFactory(filters))
        });

        return this;
    }

    public IAsyncFilterBuilder<TEntity, TFilter> AddAsync(
        Func<TFilter, Task<bool>> shouldRun,
        Func<TFilter, Task<Expression<Func<TEntity, bool>>>> filterExpressionFactory)
    {
        _filterDefinitions.Add(new AsyncFilterDefinition<TEntity, TFilter>
        {
            ShouldRun = shouldRun,
            FilterExpressionFactory = filterExpressionFactory
        });

        return this;
    }

    public IEnumerable<AsyncFilterDefinition<TEntity, TFilter>> Build() => _filterDefinitions;
}