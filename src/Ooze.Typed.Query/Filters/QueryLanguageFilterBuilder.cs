// ReSharper disable StaticMemberInGenericType

using System.Linq.Expressions;
using System.Reflection;
using Ooze.Typed.Expressions;
using Ooze.Typed.Query.Exceptions;

namespace Ooze.Typed.Query.Filters;

/// <inheritdoc />
internal class QueryLanguageFilterBuilder<TEntity> : IQueryLanguageFilterBuilder<TEntity>
{
    private static readonly Type[] SupportedTypes =
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
        ValidateExpression(dataExpression);

        var memberExpression = BasicExpressions.GetMemberExpression(dataExpression.Body);
        var propertyType = (memberExpression.Member as PropertyInfo)!.PropertyType;
        var filterName = string.IsNullOrEmpty(name)
            ? GetExpressionName(memberExpression)
            : name;
        
        ValidateName(filterName);
        
        _filterDefinitions.Add(new QueryLanguageFilterDefinition<TEntity>
        {
            Name = filterName,
            MemberExpression = memberExpression,
            PropertyType = propertyType
        });

        return this;
    }

    private void ValidateName(string name)
    {
        var nameAlreadyExists = _filterDefinitions.Any(filter => filter.Name == name);
        if (nameAlreadyExists)
        {
            throw new QueryLanguageFilterDefinitionException(
                $"Duplicate filter name detected! Name: [{name}]");
        }
    }
    
    private static string GetExpressionName(MemberExpression expression)
        => expression.Member.Name;

    private static void ValidateExpression<TProperty>(Expression<Func<TEntity, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression 
            || IsSupportedType(memberExpression.Type) == false
            || memberExpression.Member is not PropertyInfo)
        {
            throw new MemberExpressionException(
                $"Query filter expression incorrect! Please check your filter provider! Expression used: [{expression}]");
        }
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