﻿using Ooze.Typed.Filters;

namespace Ooze.Typed.Tests.Integration.Setup;

public class PostEqualFiltersProvider : IOozeFilterProvider<Post, PostFilters>
{
    public IEnumerable<IFilterDefinition<Post, PostFilters>> GetFilters()
        => Filters.Filters.CreateFor<Post, PostFilters>()
            .Equal(post => post.Id, filter => filter.Id)
            .Equal(post => post.Name, filter => filter.Name)
            .Equal(post => post.Enabled, filter => filter.Enabled)
            .Build();
}