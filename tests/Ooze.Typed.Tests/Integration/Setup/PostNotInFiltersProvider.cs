﻿using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostNotInFiltersProvider : IOozeFilterProvider<Post, PostInFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostInFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostInFilters>()
            .NotIn(post => post.Id, filter => filter.Ids)
            .NotIn(post => post.Name, filter => filter.Names)
            .NotIn(post => post.Date, filter => filter.Dates)
            .Build();
}