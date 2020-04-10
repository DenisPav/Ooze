using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;

namespace Ooze.Selections
{
    internal class OozeSelectionHandler : IOozeSelectionHandler
    {
        class FieldDefinition
        {
            public string Property { get; set; }
            public IList<FieldDefinition> Children { get; set; } = new List<FieldDefinition>();
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

            //name
            //id
            //comments
            //comments.id
            //comments.date
            //comments.users.id
            //comments.users.name

            var splitted = fields.Split(',')
                .OrderByDescending(def => def.Count(@char => @char == '.'))
                .ToList();

            IList<FieldDefinition> Parse(IEnumerable<string> fieldDefinitions, IList<FieldDefinition> existingDefinitions = null)
            {
                existingDefinitions ??= new List<FieldDefinition>();

                foreach (var fieldDefinition in fieldDefinitions)
                {
                    if (!fieldDefinition.Contains('.'))
                    {
                        if (!existingDefinitions.Any(def => def.Property.Equals(fieldDefinition, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            existingDefinitions.Add(new FieldDefinition
                            {
                                Property = fieldDefinition
                            });
                        }
                    }
                    else
                    {
                        var fieldDefinitionParts = fieldDefinition.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        var containerDef = existingDefinitions.FirstOrDefault(def => def.Property.Equals(fieldDefinitionParts[0], StringComparison.InvariantCultureIgnoreCase));

                        if (containerDef is null)
                        {
                            var newDef = new FieldDefinition
                            {
                                Property = fieldDefinitionParts[0]
                            };
                            newDef.Children = Parse(new[] { string.Join('.', fieldDefinitionParts.Skip(1)) }, newDef.Children).ToList();

                            existingDefinitions.Add(newDef);
                        }
                        else
                        {
                            containerDef.Children = Parse(new[] { string.Join('.', fieldDefinitionParts.Skip(1)) }, containerDef.Children).ToList();
                        }
                    }
                }

                return existingDefinitions;
            }

            var results = Parse(splitted);

            IEnumerable<MemberAssignment> CreateAssignments(ParameterExpression rootExpression, Type type, IEnumerable<FieldDefinition> fieldDefinitions)
            {
                var assignemnts = new List<MemberAssignment>();
                var typeProperties = type.GetProperties();

                foreach (var fieldDefinition in fieldDefinitions)
                {
                    if (!fieldDefinition.Children.Any())
                    {
                        var targetProp = typeProperties.FirstOrDefault(prop => prop.Name.Equals(fieldDefinition.Property, StringComparison.InvariantCultureIgnoreCase));

                        if (targetProp is { })
                        {
                            var bind = Bind(targetProp, MakeMemberAccess(rootExpression, targetProp));
                            assignemnts.Add(bind);
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

                            var lambda = Lambda(
                                MemberInit(
                                    New(
                                        propertyType.GetConstructors().First()
                                        ),
                                    nestedAssignments
                                    ),
                                newRootParamExpr
                                );

                            var selectCallExpr = Call(
                                typeof(Enumerable),
                                "Select",
                                new[] {
                                    propertyType,
                                    propertyType
                                },
                                MakeMemberAccess(rootExpression, targetProp),
                                lambda
                            );

                            var toListCallExpr = Call(
                                typeof(Enumerable),
                                "ToList",
                                new[] {
                                    propertyType
                                },
                                selectCallExpr
                            );

                            assignemnts.Add(Bind(
                                targetProp,
                                toListCallExpr
                                ));
                        }
                    }
                }

                return assignemnts;
            }

            var assignments = CreateAssignments(paramExpr, typeof(TEntity), results);


            var lambda = Lambda<Func<TEntity, TEntity>>(
                MemberInit(
                    New(
                        typeof(TEntity).GetConstructors().First()
                    ),
                    assignments
                ),
                paramExpr
            );

            return query.Select(lambda);
        }

        IEnumerable<MemberAssignment> CreateComplexMemberAssignments<TEntity>(ParameterExpression parentParamExpr, List<string> complexFields)
        {
            var groups = complexFields.Select(fieldDef => fieldDef.Split("."))
                .GroupBy(splitted => splitted.First())
                .ToList();

            var props = typeof(TEntity).GetProperties()
                .Where(prop => groups.Any(pair => string.Equals(prop.Name, pair.Key, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            var bindExpressions = new List<MemberAssignment>();

            foreach (var prop in props)
            {
                var propertyFields = groups.First(group => group.Key.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => x);

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    var comment = prop.PropertyType.GetGenericArguments().First();
                    var commentParam = Parameter(comment, comment.Name);

                    var commentAssignments = comment.GetProperties()
                        .Where(prop => propertyFields.Contains(prop.Name, StringComparer.InvariantCultureIgnoreCase))
                        .Select(prop => Bind(prop, MakeMemberAccess(commentParam, prop)));

                    var lambda = Lambda(
                        MemberInit(
                            New(
                                comment.GetConstructors().First()
                            ),
                        commentAssignments
                        ),
                        commentParam
                   );

                    var selectCallExpr = Call(
                        typeof(Enumerable),
                        "Select",
                        new[] {
                            comment,
                            comment
                        },
                        MakeMemberAccess(parentParamExpr, prop),
                        lambda
                    );

                    var toListCallExpr = Call(
                        typeof(Enumerable),
                        "ToList",
                        new[] {
                            comment
                        },
                        selectCallExpr
                    );

                    bindExpressions.Add(Bind(
                        prop,
                        toListCallExpr
                    ));
                }
            }

            return bindExpressions;
        }
    }
}
