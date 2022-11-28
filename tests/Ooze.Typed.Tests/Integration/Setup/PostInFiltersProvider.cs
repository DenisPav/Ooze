﻿using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostInFiltersProvider : IOozeFilterProvider<Post, PostInFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostInFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostInFilters>()
            .In(post => post.Id, filter => filter.Ids)
            .In(post => post.Name, filter => filter.Names)
            .In(post => post.Date, filter => filter.Dates)
            .Build();
}