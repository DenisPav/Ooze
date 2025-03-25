using System.Linq.Expressions;
using Ooze.Typed.Query.Exceptions;

namespace Ooze.Typed.Query.Filters;

/// <inheritdoc />
internal class QueryLanguageFilterBuilder<TEntity> : IQueryLanguageFilterBuilder<TEntity>
{
    private static Type[] SupportedTypes =
    [
        typeof(bool),
        typeof(byte),
        typeof(char),
        typeof(decimal),
        typeof(double),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(float),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(string),
        typeof(DateTime),
        typeof(DateOnly),
        typeof(Guid)
    ];

    private readonly IList<QueryLanguageFilterDefinition<TEntity>> _filterDefinitions =
        new List<QueryLanguageFilterDefinition<TEntity>>();

    public IQueryLanguageFilterBuilder<TEntity> Add<TProperty>(
        Expression<Func<TEntity, TProperty>> dataExpression,
        string? name = null)
    {
        var memberExpression = ValidateExpression(dataExpression);
        _filterDefinitions.Add(new QueryLanguageFilterDefinition<TEntity>
        {
            Name = string.IsNullOrEmpty(name)
                ? GetExpressionName(memberExpression)
                : name,
            TargetProperty = typeof(TEntity).GetProperty(GetExpressionName(memberExpression)),
        });

        return this;
    }

    private static string GetExpressionName(MemberExpression expression)
        => expression.Member.Name;

    private static MemberExpression ValidateExpression<TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression || IsSupportedType(memberExpression.Type) == false)
        {
            throw new MemberExpressionException(
                $"Query filter expression incorrect! Please check your filter provider! Expression used: [{expression}]");
        }

        return memberExpression;
    }

    private static bool IsSupportedType(Type type)
    {
        return type.IsPrimitive
               || SupportedTypes.Contains(type) 
               || type.IsEnum;
    }

    public IEnumerable<QueryLanguageFilterDefinition<TEntity>> Build()
    {
        return _filterDefinitions;
    }
}