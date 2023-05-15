using Ooze.Typed.Filters;
using Ooze.Typed.Sorters;
using Ooze.Typed.Web.Entities;

public class BlogSortersProvider : IOozeSorterProvider<Blog, BlogSorters>
{
    public IEnumerable<ISortDefinition<Blog, BlogSorters>> GetSorters()
    {
        return Sorters.CreateFor<Blog, BlogSorters>()
            .Add(blog => blog.Id, sort => sort.BlogIdSort)
            .Add(blog => blog.Name, sort => sort.BlogNameSort)
            .Build();
    }
}

public class CommentFilters
{
    public string Name { get; set; } = default!;
    public int? CommentId { get; set; } = default!;
    public IEnumerable<int> CommentIds { get; set; } = default!;
    public RangeFilter<int> CommentIdsRange { get; set; } = default!;
}

public class CommentFiltersProvider : IOozeFilterProvider<Comment, CommentFilters>
{
    public IEnumerable<IFilterDefinition<Comment, CommentFilters>> GetFilters()
    {
        return Filters.CreateFor<Comment, CommentFilters>()
            .StartsWith(comment => comment.Text, filter => filter.Name)
            .Equal(comment => comment.Id, filter => filter.CommentId)
            .In(comment => comment.Id, filter => filter.CommentIds)
            .Range(comment => comment.Post.Id, filter => filter.CommentIdsRange)
            .Build();
    }
}



public class CommentSorters
{
    public SortDirection? IdSort { get; set; }
    public SortDirection? NameSort { get; set; }
}

public class CommentsSortersProvider : IOozeSorterProvider<Comment, CommentSorters>
{
    public IEnumerable<ISortDefinition<Comment, CommentSorters>> GetSorters()
    {
        return Sorters.CreateFor<Comment, CommentSorters>()
            .Add(comment => comment.Post.Id, sort => sort.IdSort)
            .Add(comment => comment.Text, sort => sort.NameSort)
            .Build();
    }
}
