﻿using Ooze.Typed.Sorters;

namespace Ooze.Typed.Tests.MySql.OozeConfiguration;

public class PostSortersProvider : ISorterProvider<Post, PostSorters>
{
    public IEnumerable<SortDefinition<Post, PostSorters>> GetSorters()
        => Sorters.Sorters.CreateFor<Post, PostSorters>()
            .SortBy(post => post.Id, sort => sort.Id)
            .Build();
}