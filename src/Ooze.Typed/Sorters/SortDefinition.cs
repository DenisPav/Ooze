﻿using System.Linq.Expressions;

namespace Ooze.Typed.Sorters;

internal class SortDefinition<TEntity, TSorters> : ISortDefinition<TEntity, TSorters>
{
    public LambdaExpression DataExpression { get; internal set; } = null!;
    public Func<TSorters, bool> ShouldRun { get; set; } = null!;
    public Func<TSorters, SortDirection?> GetSortDirection { get; set; } = null!;
}
