using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Ooze.Selections
{
    internal class OozeSelectionHandler : IOozeSelectionHandler
    {
        public IQueryable<TEntity> Handle<TEntity>(
            IQueryable<TEntity> query,
            string fields)
            where TEntity : class
        {
            var paramExpr = Parameter(
                   typeof(TEntity),
                   typeof(TEntity).Name
            );

            var splitted = fields.Split(',');
            var simpleFields = splitted.Where(field => !field.Contains(".")).ToList();
            var complexFields = splitted.Except(simpleFields).ToList();

            var bindExpressionsSimpleFields = typeof(TEntity).GetProperties()
                .Where(prop => simpleFields.Contains(prop.Name, StringComparer.InvariantCultureIgnoreCase))
                .Select(prop => Bind(prop, MakeMemberAccess(paramExpr, prop)));

            IEnumerable<MemberAssignment> bindExpressionsComplexFields = CreateComplexMemberAssignments<TEntity>(paramExpr, complexFields);

            var lambda = Lambda<Func<TEntity, TEntity>>(
                MemberInit(
                    New(
                        typeof(TEntity).GetConstructors().First()
                    ),
                    bindExpressionsSimpleFields.Concat(bindExpressionsComplexFields)
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
                        new [] {
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
