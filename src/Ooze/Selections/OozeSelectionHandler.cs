using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Ooze.Selections
{
    internal class OozeSelectionHandler : IOozeSelectionHandler
    {
        class FieldDefinition
        {
            public string Property { get; set; }
            public IList<FieldDefinition> Children { get; set; } = new List<FieldDefinition>();

            public FieldDefinition(string property) => Property = property;
        }

        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string fields)
            where TEntity : class
        {
            var paramExpr = Parameter(
                   typeof(TEntity),
                   typeof(TEntity).Name
            );

            var splitted = fields.Split(',')
                .OrderByDescending(def => def.Count(@char => @char == '.'))
                .ToList();

            var results = Parse(splitted);
            var assignments = CreateAssignments(paramExpr, typeof(TEntity), results);


            var lambda = LambdaExpr(typeof(TEntity), paramExpr, assignments) as Expression<Func<TEntity, TEntity>>;
            return query.Select(lambda);
        }

        static IList<FieldDefinition> Parse(IEnumerable<string> fieldDefinitions, IList<FieldDefinition> existingDefinitions = null)
        {
            existingDefinitions ??= new List<FieldDefinition>();

            foreach (var fieldDefinition in fieldDefinitions)
            {
                if (!fieldDefinition.Contains('.')
                    && !existingDefinitions.Any(def => def.Property.Equals(fieldDefinition, StringComparison.InvariantCultureIgnoreCase)))
                {
                    existingDefinitions.Add(new FieldDefinition(fieldDefinition));
                }
                else
                {
                    var separatorIndex = fieldDefinition.IndexOf('.');
                    var containerPart = fieldDefinition.Remove(separatorIndex);
                    var subPart = fieldDefinition.Substring(separatorIndex + 1);
                    var containerDef = existingDefinitions.FirstOrDefault(def => def.Property.Equals(containerPart, StringComparison.InvariantCultureIgnoreCase));

                    if (containerDef is null)
                    {
                        containerDef = new FieldDefinition(containerPart);
                        existingDefinitions.Add(containerDef);
                    }

                    containerDef.Children = Parse(new[] { subPart }, containerDef.Children).ToList();
                }
            }

            return existingDefinitions;
        }

        static IEnumerable<MemberAssignment> CreateAssignments(ParameterExpression rootExpression, Type type, IEnumerable<FieldDefinition> fieldDefinitions)
        {
            var typeProperties = type.GetProperties();

            foreach (var fieldDefinition in fieldDefinitions)
            {
                if (!fieldDefinition.Children.Any())
                {
                    var targetProp = typeProperties.FirstOrDefault(prop => prop.Name.Equals(fieldDefinition.Property, StringComparison.InvariantCultureIgnoreCase));

                    if (targetProp is { })
                    {
                        var bind = Bind(targetProp, MakeMemberAccess(rootExpression, targetProp));

                        yield return bind;
                    }
                }
                else
                {
                    var targetProp = typeProperties.FirstOrDefault(prop => prop.Name.Equals(fieldDefinition.Property, StringComparison.InvariantCultureIgnoreCase));
                    if (targetProp is { } && typeof(IEnumerable).IsAssignableFrom(targetProp.PropertyType))
                    {
                        var propertyType = targetProp.PropertyType.GetGenericArguments().First();
                        var newRootParamExpr = Parameter(propertyType, targetProp.Name);

                        var nestedAssignments = CreateAssignments(newRootParamExpr, propertyType, fieldDefinition.Children);

                        var lambda = LambdaExpr(propertyType, newRootParamExpr, nestedAssignments);
                        var selectCallExpr = SelectExpr(propertyType, rootExpression, targetProp, lambda);
                        var toListCallExpr = ToListExpr(propertyType, selectCallExpr);

                        yield return Bind(targetProp, toListCallExpr);
                    }
                    else
                    {
                        //handle nested objects
                    }
                }
            }
        }

        static MethodCallExpression SelectExpr(
            Type propertyType,
            ParameterExpression paramExpr,
            PropertyInfo targetProperty,
            LambdaExpression expression)
            => Call(
                typeof(Enumerable),
                "Select",
                new[] {
                    propertyType,
                    propertyType
                },
                MakeMemberAccess(paramExpr, targetProperty),
                expression
                );

        static MethodCallExpression ToListExpr(
            Type propertyType,
            MethodCallExpression selectExpression)
            => Call(
                typeof(Enumerable),
                "ToList",
                new[] {
                    propertyType
                },
                selectExpression
                );

        static LambdaExpression LambdaExpr(
            Type propertyType,
            ParameterExpression parameterExpression,
            IEnumerable<MemberAssignment> memberAssignments)
            => Lambda(
                MemberInit(
                    New(
                        propertyType.GetConstructors().First()
                        ),
                    memberAssignments
                    ),
                parameterExpression
                );
    }
}
