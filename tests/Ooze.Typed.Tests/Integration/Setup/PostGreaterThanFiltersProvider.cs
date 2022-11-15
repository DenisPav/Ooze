﻿using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostGreaterThanFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .GreaterThan(post => post.Id, filter => filter.Id)
            .GreaterThan(post => post.Name, filter => filter.Name)
            .GreaterThan(post => post.Enabled, filter => filter.Enabled)
            .GreaterThan(post => post.Date, filter => filter.Date)
            .Build();
}